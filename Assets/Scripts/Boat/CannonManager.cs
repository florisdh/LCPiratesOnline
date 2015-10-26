﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonManager : MonoBehaviour
{
    private GameObject[] _allCannons;
    private List<GameObject> _rowOne;
    private List<GameObject> _rowTwo;

    [SerializeField]
    private float _coolDownTiming;

    private float _coolDownTimer;

    void Start()
    {
        _allCannons = GameObject.FindGameObjectsWithTag("Cannon");
        
        _rowOne = new List<GameObject>();
        _rowTwo = new List<GameObject>();

        for (int i = 0; i < _allCannons.Length; i++)
        {
            if(_allCannons[i].GetComponent<Cannon>().row == 1)
            {
                _rowOne.Add(_allCannons[i]);
            }
            else
            {
                _rowTwo.Add(_allCannons[i]);
            }
        }
    }

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (_coolDownTimer > _coolDownTiming)
            {
                fireCannons(1);
                _coolDownTimer = 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (_coolDownTimer > _coolDownTiming)
            {
                fireCannons(2);
                _coolDownTimer = 0;
            }
        }

        _coolDownTimer += Time.deltaTime;
    }

    void fireCannons(int row)
    {
        if(row == 1)
        {
            for (int i = 0; i < _rowOne.Count; i++)
            {
                _rowOne[i].GetComponent<Cannon>().Shoot();
            }
        }
        else
        {
            for (int i = 0; i < _rowTwo.Count; i++)
            {
                _rowTwo[i].GetComponent<Cannon>().Shoot();
            }            
        }
    }


}