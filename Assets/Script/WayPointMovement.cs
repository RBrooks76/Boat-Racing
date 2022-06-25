using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointMovement : MonoBehaviour
{
    private float _WaypointDistance = 0.0f;
    private float _MovementSpeed = 6f;

    private Vector3[] _Waypoints;
    private int _CurrentWaypointIndex;
    private Vector3 _CurrentWaypoint;

    private List<Transform> waypoints;
    // Start is called before the first frame update
    void Start()
    {
        _Waypoints = new Vector3[]                              //INITIALIZE WAYPOINTS
        {
            new Vector3(0, 0, 0),                               //WAYPOINT 1
            new Vector3(10, 0, 0),                              //WAYPOINT 2
            new Vector3(10, 0, 10),                             //WAYPOINT 3
            new Vector3(0, 0, 10)                               //WAYPOINT 4
        };
        _CurrentWaypointIndex = 0;                              //INITIAL WAYPOINT INDEX
        _CurrentWaypoint = _Waypoints[_CurrentWaypointIndex];   //INTIAL WAYPOINT
    }

    // Update is called once per frame
    void Update()
    {
        onWaypointMovement();
    }

    private void onWaypointMovement(){
        if (Vector3.Distance(_CurrentWaypoint, transform.position) < _WaypointDistance) //IF WAYPOINT REACHED -> UPDATE WAYPOINT
        {
            _CurrentWaypointIndex++;                                                     //INCREMENT INDEX
 
            if (_CurrentWaypointIndex < 0)                                              //CURRENT INDEX WENT NEGATIVE
                _CurrentWaypointIndex = 0;                                              //RESET WAYPOINT
 
            _CurrentWaypoint = _Waypoints[_CurrentWaypointIndex % _Waypoints.Length];   //GET NEXT WAYPOINT
        }
        else
            transform.position = Vector3.MoveTowards(                                   //MOVE TO WAYPOINT
                                transform.position,
                                _CurrentWaypoint,
                                _MovementSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()                         //DEBUGGING DISPLAY
    {
        if(_Waypoints != null)                          //IF WAYPOINTS EXIST
        {
            foreach (Vector3 waypoint in _Waypoints)    //ITERATE ALL WAYPOINTS
            {
                Gizmos.DrawSphere(waypoint, 0.1f);      //DRAW SPHERE AT CURRENT WAYPOINT
            }
        }
    }
}
