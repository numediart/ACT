using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// A class for deserializing JSON data into a list of move orders and an audio clip name.
/// </summary>
public class AlphaVer_JsonGetter : MonoBehaviour
{
    /// <summary>
    /// The JSON data to deserialize.
    /// </summary>
    public TextAsset Json;
    
    /// <summary>
    /// Represents a move order with a landmark key, movement coefficient, and time before rewind.
    /// </summary>
    [System.Serializable]
    public class Order
    {
        /// <summary>
        /// The landmark key for the move order.
        /// </summary>
        public string LandmarkKey;
        
        /// <summary>
        /// The movement coefficient for the move order.
        /// </summary>
        public Vector3 MovementCoefficient;

        public Vector3 RotationCoefficient;
        
        /// <summary>
        /// The time to wait before rewinding the move order.
        /// </summary>
        public float TimeBeforeRewind;
    }
    
    /// <summary>
    /// Represents a list of move orders and an audio clip name.
    /// </summary>
    [System.Serializable]
    public class OrderList
    {
        /// <summary>
        /// The list of move orders.
        /// </summary>
        public List<Order> MoveOrders;
        
        /// <summary>
        /// The name of the audio clip associated with the move orders.
        /// </summary>
        public string AudioClipName;
    }
    
    /// <summary>
    /// The list of move orders and audio clip name from the deserialized JSON data.
    /// </summary>
    [HideInInspector] public OrderList MyOrderList = new OrderList();
    
    /// <summary>
    /// Updates the <see cref="MyOrderList"/> field by deserializing the <see cref="Json"/> data.
    /// </summary>
    public void UpdateJson()
    {
        MyOrderList = JsonUtility.FromJson<OrderList>(Json.text);
    }
}
