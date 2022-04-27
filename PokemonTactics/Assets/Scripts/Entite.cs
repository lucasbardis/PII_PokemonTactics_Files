using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entite : MonoBehaviour
{
    [SerializeField] public int PointsDeVie;
    [SerializeField] public int PointsDeVieActuel;
    [SerializeField] public int PointsDeMouvement;
    [SerializeField] public int PointsDeMouvementActuel;
    [SerializeField] public int PointsDAction;
    [SerializeField] public int PointsDActionActuel;
    [SerializeField] public double Attaque;
    [SerializeField] public double Defense;
    [SerializeField] public int Vitesse;
    [SerializeField] public List<double> ValeursFaiblesses;
    [SerializeField] public List<string> Attaques;
    [SerializeField] public int Equipe;
    [SerializeField] public Vector3Int Position;

    public void setPosition(Vector3Int cellPosition)
    {
        this.transform.position = cellPosition + new Vector3(0.5f,0.5f,0);
        Position = cellPosition;
    }

}
