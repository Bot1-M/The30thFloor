using UnityEngine;

public class SlimeFollow : StateMachineBehaviour
{
    [SerializeField] private float velocity;
    [SerializeField] private float baseTime;
    private float timeFollows;
    private Transform player;
    private SlimeEnemy slimeEnemy;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeFollows = baseTime;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        slimeEnemy = animator.GetComponent<SlimeEnemy>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector3.MoveTowards(animator.transform.position, player.position, velocity * Time.deltaTime);
        slimeEnemy.Spin(player.position);
        timeFollows -= Time.deltaTime;
        if (timeFollows <= 0)
        {
            animator.SetTrigger("Comeback");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
