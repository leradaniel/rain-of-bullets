using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Esta clase pertenece a un Behaviour del Animator, usado generalmente para las explosiones.
public class DestroyThis : StateMachineBehaviour {
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //El Behaviour hace que cuando se entra en un stado nuevo, dentro del animator, el objeto sea destruido.
        //Para acceder al gameobject, uso al mismo animator.
        GameObject.Destroy(animator.gameObject);

    }
}
