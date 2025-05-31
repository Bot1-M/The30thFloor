using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor del sistema de turnos por rondas.
/// Mantiene la cola de unidades que participan en el combate y gestiona el flujo de turnos.
/// </summary>
public class TurnManager
{
    private Queue<ITurnTaker> turnQueue = new();
    private List<ITurnTaker> turnOrderList = new(); // NUEVO: lista original

    public event Action OnRoundStart;
    public event Action OnTurnChanged;
    public event Action OnCombatFinished;

    /// <summary>
    /// Inicializa el orden de turnos con las unidades dadas.
    /// </summary>
    /// <param name="units"> Lista de unidades que participan en el combate.</param>
    public void InitTurnOrder(List<ITurnTaker> units)
    {
        turnOrderList = new List<ITurnTaker>(units); // guardamos para reiniciar
        turnQueue = new Queue<ITurnTaker>(turnOrderList);
        OnRoundStart?.Invoke();
        ProcessNextTurn();
    }

    /// <summary>
    /// Procesa el siguiente turno. Si la cola está vacía, reinicia la ronda.
    /// Si queda una sola unidad, finaliza el combate.
    /// </summary>
    public void ProcessNextTurn()
    {
        if (turnQueue.Count == 0)
        {
            if (turnOrderList.Count <= 1)
            {
                Debug.Log("Fin del combate: solo queda el jugador.");
                OnCombatFinished?.Invoke();
                return;
            }

            Debug.Log("Fin de la ronda, reiniciando turno.");
            turnQueue = new Queue<ITurnTaker>(turnOrderList);
            OnRoundStart?.Invoke();
        }

        if (turnQueue.Count > 0)
        {
            ITurnTaker current = turnQueue.Dequeue();
            OnTurnChanged?.Invoke();
            current.StartTurn(OnTurnFinished);
        }
    }


    private void OnTurnFinished()
    {
        ProcessNextTurn();
    }

    /// <summary>
    /// Elimina una unidad del sistema de turnos.
    /// Recalcula la cola de turnos actual excluyendo dicha unidad.
    /// </summary>
    /// <param name="taker">Unidad a eliminar del sistema de turnos.</param>
    public void RemoveTurnTaker(ITurnTaker taker)
    {
        // Elimina de la lista original
        turnOrderList.Remove(taker);

        // Volvemos a crear la cola sin esa unidad
        Queue<ITurnTaker> newQueue = new Queue<ITurnTaker>();
        foreach (var t in turnQueue)
        {
            if (t != taker)
                newQueue.Enqueue(t);
        }

        turnQueue = newQueue;

        Debug.Log("Unidad eliminada de los turnos.");
    }

}

/// <summary>
/// Interfaz que deben implementar las entidades que participan en el sistema de turnos.
/// </summary>
public interface ITurnTaker
{
    /// <summary>
    /// Inicia el turno de esta unidad.
    /// </summary>
    /// <param name="onTurnComplete">Acción que debe llamarse al finalizar el turno.</param>
    void StartTurn(Action onTurnComplete);
}

