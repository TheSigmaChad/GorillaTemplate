using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class TransformToDetectedItem : MonoBehaviour
{
    // Transformable transformable

    private void Update()
    {
        /* if (cooldown is not active)

         * if (trigger is pressed)
         * { 
         * if (transformable is not null AND cooldown is not active)
         *      then transform into object
         *     

        * if (hand is hovering over object AND cooldown is not active)
         * then give object glow material
         */


    }

    private void OnTriggerEnter(Collider other)
    {
        //if (sphere trigger enters object)
        //check if object has Transformable component
        //if (other.TryGetComponent<Transformable>(out transformable))
        //set transformable reference to Transformable Component
        //then activate object selection
    }

    private void OnTriggerExit(Collider other)
    {
        //if (sphere trigger exits object)
        //deactivate object selection
    }

}
