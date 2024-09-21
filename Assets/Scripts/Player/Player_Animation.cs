using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    [SerializeField] TP_Player_Movement tP_Player_Movement;
    [SerializeField] Animator Player_Animator;
    

    private void Update() 
    {
        transform.localPosition = Vector3.zero;// to stay model within player// 


       
        if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Crouch_Idle)
        {
            Player_Animator.SetBool("is_Crouch",true);
            Player_Animator.SetFloat("Crouch_Blend",0.01f,0.2f,Time.deltaTime);
            
        }
        else if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Crouch_Walk)
        {
            Player_Animator.SetBool("is_Crouch",true);
            Player_Animator.SetFloat("Crouch_Blend",1f,0.1f,Time.deltaTime);
        }
        else if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Jump)
        {
            Player_Animator.SetBool("isJump",true);
            
        }
        else if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Idle)
        {
            Player_Animator.SetBool("is_Crouch",false);
            Player_Animator.SetBool("isJump",false);
            Player_Animator.SetFloat("Walk_Idle_Blend",0.01f,0.2f,Time.deltaTime);

        }
        else if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Walking)
        {
            Player_Animator.SetBool("is_Crouch",false);
            Player_Animator.SetBool("isJump",false);
            Player_Animator.SetFloat("Walk_Idle_Blend",1f,0.1f,Time.deltaTime);
        }

        if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Sprinting)
        {
            Player_Animator.SetBool("isJump",false);
            Player_Animator.SetBool("isRun",true);
        }
        else if(tP_Player_Movement.playerState == TP_Player_Movement.PlayerState.Jump)
        {
            Player_Animator.SetBool("isJump",true);
        }
        else
        {
            Player_Animator.SetBool("isRun",false);
        }

        
    }


    
}
