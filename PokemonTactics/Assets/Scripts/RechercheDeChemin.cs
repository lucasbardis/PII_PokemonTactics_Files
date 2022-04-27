using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RechercheDeChemin : MonoBehaviour
{

    public CommandeGrille commandesDeJeu;
    [SerializeField] public List<Noeud> NoeudsOuverts;
    private List<int[]> ancienChemin = null;

    public int calculChemin(Vector3Int cellPosition, int portee, bool inAttack = false)
    {
        Noeud arrivee = new Noeud(cellPosition.x,cellPosition.y);
        int rayon = 0;
        Noeud depart = new Noeud(commandesDeJeu.entite.Position.x, commandesDeJeu.entite.Position.y);
        NoeudsOuverts = new List<Noeud>() {depart};
        Noeud bonNoeud = null;

        while(rayon != portee) //On ouvre la recherche à toutes les cases à portée
        {
            List<Noeud> successeursTempo = new List<Noeud>();
            foreach (Noeud noeud in NoeudsOuverts)
            {
                List<Noeud> successeurs = noeud.getSuccesseur();
                foreach (Noeud succ in successeurs)
                {
                    successeursTempo = VerifAddition(succ, successeursTempo); //Si il n'a pas déjà été ajouté, on l'ajoute
                }
            }
            NoeudsOuverts.AddRange(successeursTempo);
            bonNoeud = VerifCible(arrivee);
            if (bonNoeud != null)
                break; //Si le noeud cherché n'est pas dans les noeuds ouverts, on arrête la recherche, c'est pas à portée
            rayon++;
        }

        List<int[]> bonnesCases = new List<int[]>();

        while(bonNoeud != null)
        {
            bonnesCases.Add(new int[] {bonNoeud.X, bonNoeud.Y});
            bonNoeud = bonNoeud.precedent; //On ajoute tous les noeuds qui ont menés au noeud recherché, pour obtenir le chemin
        }

        if (inAttack) { //Si c'est une attaque, on s'en fiche du chemin
            if (bonnesCases.Count != 0)
                return bonnesCases.Count;
            else
                return -1;
        } else { //Sinon on colorie le chemin
            if (bonnesCases.Count != 0) {
                ColorieChemin(bonnesCases);
                return bonnesCases.Count;
            } else {
                ResetColor();
                return -1;
            }
        }

    }

    Noeud VerifCible(Noeud arrivee)
    {
        Noeud bonNoeud = null;
        foreach (Noeud noeud in NoeudsOuverts)
        {
            if (noeud.X == arrivee.X && noeud.Y == arrivee.Y)
                bonNoeud=noeud;
        }
        
        return bonNoeud;
    }

    void ColorieChemin(List<int[]> bonnesCases)
    {
        ResetColor();
        Vector3Int cellPosition = new Vector3Int(0,0,0);
        foreach (var item in bonnesCases)
        {
            cellPosition.x = item[0];
            cellPosition.y = item[1];
            commandesDeJeu.grilleTactique.SetTileFlags(cellPosition, TileFlags.None);
            commandesDeJeu.grilleTactique.SetColor(cellPosition, Color.green);
        }

        ancienChemin = bonnesCases;
        
    }

    void ResetColor()
    {
        Vector3Int cellPosition = new Vector3Int(0,0,0);
        if (ancienChemin != null){
            foreach (var item in ancienChemin)
            {
                cellPosition.x = item[0];
                cellPosition.y = item[1];
                commandesDeJeu.grilleTactique.SetTileFlags(cellPosition, TileFlags.None);
                commandesDeJeu.grilleTactique.SetColor(cellPosition, Color.white);
            }
        }
    }

    List<Noeud> VerifAddition(Noeud succ, List<Noeud> successeursTempo)
    {
        bool verif = false;
        Vector3Int position = new Vector3Int(succ.X,succ.Y,0);
        Tile caseVerif = commandesDeJeu.grilleTactique.GetTile<Tile>(position);
        if (caseVerif != null) {
        if (caseVerif.name.ToString()=="CaseTactique")
            verif = true;
        }
        if(verif)
        {
            foreach (Noeud noeud in NoeudsOuverts)
            {
                if (noeud.X == succ.X && noeud.Y == succ.Y)
                    verif = false;
            }
            foreach (Noeud noeud in successeursTempo)
            {
                if (noeud.X == succ.X && noeud.Y == succ.Y)
                    verif = false;
            }
        }

        if (verif)
            successeursTempo.Add(succ);

        return successeursTempo;
    }


}
