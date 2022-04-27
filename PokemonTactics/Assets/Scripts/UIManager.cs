using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text affichageJoueur;
    public Text affichageAutre;
    public Text affichageNbTour;
    public Button boutonAttaque;
    public Text log;
    public DeroulementDuJeu jeu;
    public Image aidePlacement;
    public Text finDeTour;

    void Start()
    {
        for (int i = 0; i < jeu.entites.Count; i++) //On crée les images et couleurs des Pokémons
        {
            GameObject panel = new GameObject($"Panel{jeu.entites[i].name}");
            RectTransform trans = panel.AddComponent<RectTransform>();
            trans.transform.SetParent(aidePlacement.transform);
            trans.anchoredPosition = new Vector3(i*100,0,0);
            trans.localScale = new Vector3(1,1,1);
            Image couleur = panel.AddComponent<Image>();
            if (jeu.entites[i].Equipe == 0)
                couleur.color = new Color32(255,0,0,100);
            else
                couleur.color = new Color32(0,0,255,100);

            GameObject icon = new GameObject($"Icone{jeu.entites[i].name}");
            trans = icon.AddComponent<RectTransform>();
            trans.transform.SetParent(aidePlacement.transform);
            trans.anchoredPosition = new Vector3(i*100,0,0);
            trans.localScale = new Vector3(1,1,1);
            Image image = icon.AddComponent<Image>();
            var spritePokemon = Resources.Load<Sprite>($"Sprites/{jeu.entites[i].name}/presentation");
            image.sprite = spritePokemon;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        //On change les stats selon l'entité qui joue
        affichageJoueur.text = jeu.entiteJoueuse.name + " :\nPV : " + jeu.entiteJoueuse.PointsDeVieActuel.ToString() + "/" + jeu.entiteJoueuse.PointsDeVie.ToString() 
        + "\nPM : "+ jeu.entiteJoueuse.PointsDeMouvementActuel.ToString() + "/" + jeu.entiteJoueuse.PointsDeMouvement.ToString()
        + "\nPA : " + jeu.entiteJoueuse.PointsDActionActuel.ToString() + "/" + jeu.entiteJoueuse.PointsDAction.ToString() 
        + "\nVitesse : " + jeu.entiteJoueuse.Vitesse.ToString() + "\nAttaque : " + jeu.entiteJoueuse.Attaque.ToString() + "\nDéfense : " + jeu.entiteJoueuse.Defense.ToString();

        if (jeu.commandesDeJeu.occupied == null)
        {
            affichageAutre.text = " ";
        }
        else //Si on vise une autre entité on fait pareil
        {
            affichageAutre.text = jeu.commandesDeJeu.occupied.name + " :\nPV : " + jeu.commandesDeJeu.occupied.PointsDeVieActuel.ToString() + "/" + jeu.commandesDeJeu.occupied.PointsDeVie.ToString() 
            + "\nPM : "+ jeu.commandesDeJeu.occupied.PointsDeMouvementActuel.ToString() + "/" + jeu.commandesDeJeu.occupied.PointsDeMouvement.ToString()
            + "\nPA : " + jeu.commandesDeJeu.occupied.PointsDActionActuel.ToString() + "/" + jeu.commandesDeJeu.occupied.PointsDAction.ToString() 
            + "\nVitesse : " + jeu.commandesDeJeu.occupied.Vitesse.ToString() + "\nAttaque : " + jeu.commandesDeJeu.occupied.Attaque.ToString() + "\nDéfense : " + jeu.commandesDeJeu.occupied.Defense.ToString();
        }

        if (!jeu.placement) //Affichage du nombre de tour
            affichageNbTour.text = "Placement";
        else
            affichageNbTour.text = jeu.nbTour.ToString();

    }

    public void KO(Entite entite)
    {
        log.text += "\n"+entite.name+" est KO !";

        GameObject panel = GameObject.Find($"Panel{entite.name}");
        panel.GetComponent<Image>().color = new Color32(0,0,0,200);
    }

    public void ChangerAttaque()
    {
        boutonAttaque.GetComponentInChildren<Text>().text = jeu.entiteJoueuse.Attaques[0];
        GameObject attaqueObjet = GameObject.Find($"{jeu.entiteJoueuse.Attaques[0]}");
        Attaque attaque = attaqueObjet.GetComponent<Attaque>();
        boutonAttaque.onClick.AddListener(delegate {jeu.Attaquer(attaque);});
    }

}
