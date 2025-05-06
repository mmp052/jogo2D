using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public class State : MonoBehaviour{

        public List<Transition> transitions;

        public virtual void Awake() 
        {
            transitions = new List<Transition>();
            //TODO// definir as transições aqui}
        }

        public virtual void OnEnable()
        {
            //TODO// Inicializações do estado;}
        }

        public virtual void OnDisable()
        {
            //TODO//Finalização do estado}
        }

        public virtual void Update()
        {
            //TODO// Implementação do comportamento}
        }

        public virtual void LateUpdate()
        {
            foreach (Transition t in transitions) {
                // Para cada transição que esse estado tiver// é feita a verificação de sua condiçãoforeach (Transition t in transitions) {
                if (t.condition.Test()) 
                {
                    t.target.enabled = true;
                    this.enabled = false;
                    return;
                }
            }
        }

    }
}