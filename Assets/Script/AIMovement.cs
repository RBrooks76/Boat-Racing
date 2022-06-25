using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent _navAgent;

    [SerializeField]
    private List<Transform> _waypoints;
    private int _target1 = 0;
    private int _target2 = 0;
    private int _target3 = 0;
    private int _target4 = 0;
    private Vector3 axis;
    private Vector3 axis1 = new Vector3(0, 0, 8);
    private Vector3 axis2 = new Vector3(0, 0, 5);
    private Vector3 axis3 = new Vector3(0, 0, 0);
    private Vector3 axis4 = new Vector3(0, 0, -5);
    // Start is called before the first frame update
    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        
        if(_navAgent is null)
            Debug.LogError("Nav Mesh Agent is NULL");
        if(_waypoints.Count > 1 && !(_waypoints[0] is null)){

            if(_target1 == 0)
                axis = axis1;

            if(_target2 == 0)
                axis = axis2;

            if(_target3 == 0)
                axis = axis3;

            if(_target4 == 0)
                axis = axis4;

            _navAgent.SetDestination(_waypoints[0].position + axis);
        } 
        else 
            Debug.LogError("Please select 2 waypoints at least");
    }

    // Update is called once per frame
    void Update()
    {
        // Vector3.MoveTowards(this.transform.position, _waypoints[_target].position, Time.deltaTime * speed);
        // this.transform.position += Vector3.right * Time.deltaTime * speed;
    }

    // private void OnTurn(int cnt, Vector3 ){

    // }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("waypoint") && this.CompareTag("boat1")){
            _target1++;
            if(_target1 == _waypoints.Count){
                Debug.Log("end");
                // _waypoints.Reverse();
                _target1 = 0;
            }

            // Debug.Log( "Boat1 : " + _waypoints[_target1].name);

            var variable = new Vector3(0, 0, 10);
            if(!(_waypoints[_target1] is null)) {

                if(_target1 == 15 || _target1 == 16 || _target1 == 25 || _target1 == 26 || _target1 == 33){
                    _navAgent.speed = 6;
                    Debug.Log( _waypoints[_target1].name);
                    variable = new Vector3(0, 0, 0);
                } else {
                    _navAgent.speed = 10;
                    variable = new Vector3(0, 0, 10);
                }

                if(_target1 == 30)
                    variable = new Vector3(0, 0, 0);
                else 
                    variable = new Vector3(0, 0, 10);

                var position = _waypoints[_target1].position + variable;
                _navAgent.SetDestination(position);

            }
            else 
                Debug.LogError("Target waypoint is null");
        }   

        if(other.CompareTag("waypoint") && this.CompareTag("boat2")){
            _target2++;
            if(_target2 == _waypoints.Count){
                Debug.Log("end");
                // _waypoints.Reverse();
                _target2 = 1;
            }

            // Debug.Log( "boat2 : " + _waypoints[_target2].name);

            if(!(_waypoints[_target2] is null)) {
                var variable = new Vector3(0, 0, 5);

                if(_target2 == 15 || _target2 == 16 || _target2 == 25 || _target2 == 26 || _target2 == 33){
                    _navAgent.speed = 6;
                    Debug.Log( _waypoints[_target2].name);
                    variable = new Vector3(0, 0, 0);
                } else {
                    _navAgent.speed = 10;
                    variable = new Vector3(0, 0, 5);
                }

                if(_target2 == 30)
                    variable = new Vector3(0, 0, 0);
                else 
                    variable = new Vector3(0, 0, 5);

                var position = _waypoints[_target2].position + variable;
                _navAgent.SetDestination(position);
            }
            else 
                Debug.LogError("Target waypoint is null");
        }   

        if(other.CompareTag("waypoint") && this.CompareTag("boat3")){
            _target3++;
            if(_target3 == _waypoints.Count){
                Debug.Log("end");
                // _waypoints.Reverse();
                _target3 = 1;
            }

            // Debug.Log( "boat3 : " + _waypoints[_target3].name);

            if(!(_waypoints[_target3] is null)) {
                var variable = new Vector3(0, 0, 0);

                if(_target3 == 15 || _target3 == 16 || _target3 == 25 || _target3 == 26 || _target3 == 33){
                    _navAgent.speed = 6;
                    Debug.Log( _waypoints[_target3].name);
                    variable = new Vector3(0, 0, 0);
                } else {
                    _navAgent.speed = 10;
                    variable = new Vector3(0, 0, 5);
                }

                if(_target3 == 30)
                    variable = new Vector3(0, 0, 0);
                else 
                    variable = new Vector3(0, 0, 5);

                var position = _waypoints[_target3].position + variable;
                _navAgent.SetDestination(position);
            }
            else 
                Debug.LogError("Target waypoint is null");
        }  

        if(other.CompareTag("waypoint") && this.CompareTag("boat4")){
            _target4++;
            if(_target4 == _waypoints.Count){
                Debug.Log("end");
                // _waypoints.Reverse();
                _target4 = 1;
            }

            // Debug.Log( "boat4 : " + _waypoints[_target4].name);

            if(!(_waypoints[_target4] is null)) {
                var variable = new Vector3(0, 0, -5);

                if(_target4 == 15 || _target4 == 16 || _target4 == 25 || _target4 == 26 || _target4 ==33){
                    _navAgent.speed = 6;
                    Debug.Log( _waypoints[_target4].name);
                    variable = new Vector3(0, 0, 0);
                } else {
                    _navAgent.speed = 10;
                    variable = new Vector3(0, 0, 5);
                }

                if(_target4 == 30)
                    variable = new Vector3(0, 0, 0);
                else 
                    variable = new Vector3(0, 0, 5);

                var position = _waypoints[_target4].position + variable;
                _navAgent.SetDestination(position);
            }
            else 
                Debug.LogError("Target waypoint is null");
        } 
    }
}
