using Dennis.Tools.DialogueGraph.Data;
using UnityEngine;

namespace Dennis.Tools.DialogueGraph.Sample
{

    public class Item : MonoBehaviour, IPickupable
    {
        public string itemName;

        public void OnPickup()
        {
            VariableManager.ApplyOperation(new VariableOperationData
            {
                VariableName = itemName,
                OperationType = VariableOperationType.SetTrue,
            });

            Destroy(gameObject);
        }
    }
}
