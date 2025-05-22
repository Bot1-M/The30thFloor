using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    private Queue<ITurnTaker> turnQueue = new();
    private List<ITurnTaker> turnOrderList = new(); // NUEVO: lista original
    private bool isProcessingTurn = false;

    public event Action OnRoundStart;
    public event Action OnTurnChanged;
    public event Action OnCombatFinished;

    public void InitTurnOrder(List<ITurnTaker> units)
    {
        turnOrderList = new List<ITurnTaker>(units); // guardamos para reiniciar
        turnQueue = new Queue<ITurnTaker>(turnOrderList);
        isProcessingTurn = false;
        OnRoundStart?.Invoke();
        ProcessNextTurn();
    }

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
            isProcessingTurn = true;
            OnTurnChanged?.Invoke();
            current.StartTurn(OnTurnFinished);
        }
    }


    private void OnTurnFinished()
    {
        isProcessingTurn = false;
        ProcessNextTurn();
    }

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

public interface ITurnTaker
{
    void StartTurn(Action onTurnComplete);
}

//using UnityEngine;

//public class TurnManager
//{
//    private int m_TurnCount;

//    public event System.Action OnTick;

//    public TurnManager()
//    {
//        m_TurnCount = 1;
//    }

//    public void Tick()
//    {
//        m_TurnCount += 1;
//        Debug.Log("Current turn count : " + m_TurnCount);
//        OnTick?.Invoke();
//    }

//}
