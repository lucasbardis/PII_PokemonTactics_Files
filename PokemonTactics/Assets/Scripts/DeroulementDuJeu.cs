using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DeroulementDuJeu : MonoBehaviour
{
    [SerializeField] public List<string> Types = new List<string>() {"Normal","Feu","Eau","Plante","Electrik","Glace","Combat","Poison","Sol","Vol","Psy","Insecte","Roche","Spectre","Dragon","Tenebres","Acier"};
    public List<Entite> entites;
    public List<Attaque> attaques;
    public UIManager interfaceDeJeu;
    public CommandeGrille commandesDeJeu;
    public Entite entiteJoueuse;
    public int nbTour = 0;
    private int tourFictif = 0; //permet de passer un tour quand une entité est KO
    public bool placement = false;

    // Start is called before the first frame update
    void Start()
    {
        //On crée les entites
        string path = Application.dataPath + Path.AltDirectorySeparatorChar + "teamData.txt";
        StreamReader reader = new StreamReader(path); 
        string rouge = reader.ReadLine();
        string bleu = reader.ReadLine();
        string[] teamRouge = rouge.Split('.');
        string[] teamBleu = bleu.Split('.');
        for (int i = 1; i < teamRouge.Length; i++)
        {
            GameObject newPokemon = new GameObject($"{teamRouge[i]}");
            Entite composantEntite = newPokemon.AddComponent<Entite>();
            composantEntite.Equipe = 0;
            entites.Add(composantEntite);
            SpriteRenderer sr = newPokemon.AddComponent<SpriteRenderer>();
            var texture = Resources.Load<Sprite>($"Sprites/{teamRouge[i]}/sheet");
            sr.sprite = texture;
            newPokemon.SetActive(false);
        }
        for (int i = 1; i < teamBleu.Length; i++)
        {
            GameObject newPokemon = new GameObject($"{teamBleu[i]}");
            Entite composantEntite = newPokemon.AddComponent<Entite>();
            composantEntite.Equipe = 1;
            entites.Add(composantEntite);
            SpriteRenderer sr = newPokemon.AddComponent<SpriteRenderer>();
            var texture = Resources.Load<Sprite>($"Sprites/{teamBleu[i]}/sheet");
            sr.sprite = texture;
            newPokemon.SetActive(false);
        }
        reader.Close();


        //On initialise les attaques
        for (int i = 0; i < attaques.Count; i++)
        {
            var data = Resources.Load<TextAsset>($"Attaques/{attaques[i].name}");
            JsonUtility.FromJsonOverwrite(data.ToString(),attaques[i]);
        }

        //On initialise les entités
        for (int i = 0; i < entites.Count; i++)
        {
            var data = Resources.Load<TextAsset>($"Sprites/{entites[i].gameObject.name}/data");
            JsonUtility.FromJsonOverwrite(data.ToString(),entites[i]);
            entites[i].PointsDActionActuel = entites[i].PointsDAction;
            entites[i].PointsDeMouvementActuel = entites[i].PointsDeMouvement;
            entites[i].PointsDeVieActuel = entites[i].PointsDeVie;
        }
        entites = entites.OrderByDescending(o=>o.Vitesse).ToList();
        entiteJoueuse=entites[0];
        commandesDeJeu.entite = entites[0];
    }

    public void Attaquer(Attaque attaque)
    {
        if (!placement)
            return;
        else {
            if (entiteJoueuse.PointsDActionActuel >= attaque.Cout)
                commandesDeJeu.inAttack = attaque;
            else {
                interfaceDeJeu.log.text += "\nPas assez de points d'action !";
            }
        }
    }

    public void CalculDegat(Entite frappeur, Entite victime, Attaque attaque)
    {
        
        int idx = Types.IndexOf(attaque.Type);
        //Calcul des dégâts : (Puissance * (Atq/Def))/2 * Résistance
        int degat = Convert.ToInt32((attaque.Puissance * (frappeur.Attaque / victime.Defense))/2.0 * victime.ValeursFaiblesses[idx]);
        interfaceDeJeu.log.text += "\n" + frappeur.name + " a touché " + victime.name + " avec " + attaque.name + " pour " + degat + "dégâts";
        victime.PointsDeVieActuel -= degat;
        if (victime.PointsDeVieActuel <= 0 )
            {
                victime.setPosition(new Vector3Int(100,100,0));
                interfaceDeJeu.KO(victime);
            }
    }

    public void FinDeTour()
    {
        if (!placement) {
            //Empêche de finir le tour si on a pas placé le Pokemon
            if (entiteJoueuse.Position == new Vector3Int(100,100,0))
                return;

            //Finit le placement quand la dernière entité a été placée
            if (entiteJoueuse == entites[entites.Count -1]) {
                interfaceDeJeu.finDeTour.text = "Fin du tour";
                placement = true;
            }
        }        
        else {
            bool[] equipes = new bool[] {false, false}; //Verifie la victoire d'une équipe

            foreach (Entite entite in entites)
            {
                entite.PointsDActionActuel=entite.PointsDAction;
                entite.PointsDeMouvementActuel=entite.PointsDeMouvement;
                if (entite.PointsDeVieActuel > 0)
                    equipes[entite.Equipe] = true;
            }

            if (!equipes[0])
                FinDuJeu(1);
            else if (!equipes[1])
                FinDuJeu(0);

            nbTour++;
        }

        tourFictif++; //permet de sauter un tour si une entité est KO
        entiteJoueuse = entites[tourFictif%entites.Count];
        while(entiteJoueuse.PointsDeVieActuel <= 0)
        {
            tourFictif++;
            entiteJoueuse = entites[tourFictif%entites.Count];
        }
        commandesDeJeu.entite = entiteJoueuse;
        commandesDeJeu.inAttack = null;
        
        interfaceDeJeu.ChangerAttaque();
    }

    public void FinDuJeu(int equipe)
    {
        if (equipe == 0)
            interfaceDeJeu.log.text += "\n Victoire de l'équipe Rouge ! ";
        else
            interfaceDeJeu.log.text += "\n Victoire de l'équipe Bleu ! ";
    }

    public void RetourMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
