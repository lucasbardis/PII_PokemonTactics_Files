using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CommandeGrille : MonoBehaviour
{

    [SerializeField] public Tilemap grilleTactique;
    [SerializeField] public Tilemap grilleGraphique;
    [SerializeField] public Tilemap grilleDetails;
    [SerializeField] public string Coordonnees = "";
    [SerializeField] public string InfosCase = "";
    [SerializeField] public string InfosEntite = "zaza";
    [SerializeField] public Entite occupied;
    [SerializeField] public Attaque inAttack;
    [SerializeField] public Entite entite;
    [SerializeField] public DeroulementDuJeu jeu;
    [SerializeField] public RechercheDeChemin recherche;
    private Vector3Int cellPosition;
    private Vector3Int precedentePos;
    private int depensePM;
    private int portee;
    private List<Vector3Int> oldZoneFrappe = null;

    // Update is called once per frame
    void Update()
    {
        Hover();
        if (Input.GetKey(KeyCode.Escape)) //On annule l'attaque
            inAttack = null;

        if (Input.GetMouseButtonDown(0)) //Si on clique droit
        {
            if (InfosCase=="CaseTactique" && occupied == null && inAttack == null)
            {
                if (depensePM != -2) { //Permet d'éviter de perdre des PM lors du placement
                    entite.setPosition(cellPosition);
                    entite.PointsDeMouvementActuel -= depensePM;
                    depensePM = 0;
                    if (!jeu.placement) //On affiche le rendu du sprite si on vient de placer l'entité
                        entite.gameObject.SetActive(true);
                }

            } else if (InfosCase=="CaseTactique" && inAttack != null) { //Si on attaque
                entite.PointsDActionActuel -= inAttack.Cout;
                List<Entite> entitesTouchees = new List<Entite>();
                foreach (Vector3Int cell in oldZoneFrappe)
                {
                    foreach (Entite entite in jeu.entites)
                    {
                        if (entite.Position == cell)
                            entitesTouchees.Add(entite);
                    }
                }
                foreach (Entite entiteTouchee in entitesTouchees)
                {
                    jeu.CalculDegat(entite,entiteTouchee,inAttack);
                }
                inAttack = null;
            }
        }
        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal")) //On bouge la caméra
        {
            if (Camera.main.transform.position.x >= 5 && Input.GetAxis("Horizontal")>0)
                Debug.Log("Bord du terrain atteint");
            else if (Camera.main.transform.position.x <= -5 && Input.GetAxis("Horizontal")<0)
                Debug.Log("Bord du terrain atteint");
            else if (Camera.main.transform.position.y >= 5 && Input.GetAxis("Vertical")>0)
                Debug.Log("Bord du terrain atteint");
            else if (Camera.main.transform.position.y <= -5 && Input.GetAxis("Vertical")<0)
                Debug.Log("Bord du terrain atteint");
            else
                Camera.main.transform.position += new Vector3(Input.GetAxis("Horizontal")/10,Input.GetAxis("Vertical")/10,0); //On change la position de la caméra, la division permet de réduire la vitesse
        }
    }

    void Hover()
    {
        Vector3 mousePos = Input.mousePosition; //Coordonnées de la souris sur l'écran
        Vector3 posOnWorld = Camera.main.ScreenToWorldPoint(mousePos); //Transformées aux coordonnées dans le monde
        cellPosition = grilleTactique.WorldToCell(posOnWorld); //Transformées aux coordonnées dans la grilleTactique
        Coordonnees = cellPosition.x + " | " + cellPosition.y ;
        Tile caseSurvolee = grilleTactique.GetTile<Tile>(cellPosition);

        if (caseSurvolee != null)
        {
            InfosCase =caseSurvolee.name.ToString();
            occupied = null;

            foreach (Entite entite in jeu.entites) //On cherche si on survole une entité
            {
                if (entite.Position == cellPosition && entite.gameObject.activeSelf == true)
                    occupied = entite;
            }

            grilleTactique.SetTileFlags(cellPosition, TileFlags.None); //Autorise le changement de couleur

            if (InfosCase=="CaseTactique" && (occupied == null || occupied != entite))
            {   

                if (inAttack != null) { //Si on attaque

                    if (oldZoneFrappe != null){  //On rétablit la couleur de l'ancienne zone de frappe
                    foreach (Vector3Int cell in oldZoneFrappe)
                    {
                        grilleTactique.SetTileFlags(cell, TileFlags.None);
                        grilleTactique.SetColor(cell, Color.white);
                    }}

                    portee = recherche.calculChemin(cellPosition,inAttack.Portee, true); //On recherche si on est à portée
                    if (portee >= 0) {
                        List<Vector3Int> zoneFrappe = inAttack.FormeAttaque(cellPosition, entite.Position, this); //Calcule de la nouvelle zone de frappe
                        zoneFrappe.Add(cellPosition); 
                        foreach (Vector3Int cell in zoneFrappe)
                        {
                            if (grilleTactique.GetTile<Tile>(cell)!=null){ 
                                if (grilleTactique.GetTile<Tile>(cell).name.ToString()=="CaseTactique"){
                                    grilleTactique.SetTileFlags(cell, TileFlags.None);
                                    grilleTactique.SetColor(cell, Color.blue);
                                }
                            }
                        }
                        oldZoneFrappe = zoneFrappe;

                    } else
                        grilleTactique.SetColor(cellPosition, Color.red); //Pas à portée --> colorie en rouge

                //Si on attaque pas :
                } else if (!jeu.placement) { //Si on est en phase de placement
                    grilleTactique.SetColor(cellPosition, Color.green);

                } else { //Sinon Calcul du chemin
                    depensePM = recherche.calculChemin(cellPosition,entite.PointsDeMouvementActuel) - 1;
                    if (depensePM == -2)
                        grilleTactique.SetColor(cellPosition, Color.red);
                }
            } else {
                    grilleTactique.SetColor(cellPosition, Color.red);
            }


        }
        else
            InfosCase="Hors de la grilleTactique";

        if (precedentePos != cellPosition) //On rétablit la couleur de la case précédente
        {
            grilleTactique.SetTileFlags(precedentePos, TileFlags.None);
            grilleTactique.SetColor(precedentePos, Color.white);
        }
        precedentePos = cellPosition;
    }

    public void ChangerVisibilite(int choix)
    {
        if (choix == 0) { //Combiné
            grilleGraphique.GetComponent<Renderer>().enabled= true;
            grilleTactique.GetComponent<Renderer>().enabled= true;
            grilleDetails.GetComponent<Renderer>().enabled= true;
        }
        else if (choix == 1) { //Vue tactique
            grilleGraphique.GetComponent<Renderer>().enabled= false;
            grilleTactique.GetComponent<Renderer>().enabled= true;
            grilleDetails.GetComponent<Renderer>().enabled= false;
        }
        else if (choix == 2) { //Vue Graphique
            grilleGraphique.GetComponent<Renderer>().enabled= true;
            grilleTactique.GetComponent<Renderer>().enabled= false;
            grilleDetails.GetComponent<Renderer>().enabled= true;
        }
    }

}
