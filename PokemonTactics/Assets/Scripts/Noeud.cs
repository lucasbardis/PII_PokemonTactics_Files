using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noeud
{
    public int X;
    public int Y;
    public int valeur = 0;
    public Noeud precedent;

    public Noeud(int x, int y) { X=x; Y=y;}
    public Noeud(int x, int y, Noeud noeud) { X=x; Y=y; precedent=noeud; valeur=noeud.valeur+1;}
    

    public List<Noeud> getSuccesseur()
    {
        //Il y a 4 successeurs :
        Noeud nHaut = new Noeud(X,Y+1,this);
        Noeud nBas = new Noeud(X,Y-1,this);
        Noeud nGauche = new Noeud(X-1,Y,this);
        Noeud nDroite = new Noeud(X+1,Y,this);

        List<Noeud> successeurs = new List<Noeud>() {nHaut, nBas, nGauche, nDroite};
        return successeurs;

    } 

    public override string ToString()
    {
        string affichage = X.ToString() + " | " + Y.ToString() + " | Valeur : " + valeur.ToString();
        if (precedent != null)
            affichage += " | Précedent : " + precedent.X.ToString() + " | " + precedent.Y.ToString();
        return affichage;
    }
}
