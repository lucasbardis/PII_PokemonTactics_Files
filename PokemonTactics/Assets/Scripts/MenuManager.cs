using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MenuManager : MonoBehaviour
{
    public string combat; //Nom de la scène à charger
    public GameObject choixEquipe; //Fenêtre de choix d'équipe
    public List<string> PokemonsDispo = new List<string>() {"Pikachu","Florizarre","Dracaufeu","Tortank"}; //Liste des pokémons à charger
    [SerializeField] public string rouge = ""; //Pokémons dans l'équipe rouge
    [SerializeField] public string bleu = "";
    public List<string> TeamRouge = new List<string>() {};
    public List<string> TeamBleu = new List<string>() {};
    public GameObject emplacement; //Emplacement dans l'UI des Pokémons
    public Button boutonRouge;
    public Button boutonBleu;
    public GameObject titreRouge;
    public GameObject titreBleu;

    public void LancerPartie() { //on écrit la data des équipes puis on change de scène
        string path = Application.dataPath + Path.AltDirectorySeparatorChar + "teamData.txt";
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(rouge);
        writer.WriteLine(bleu);
        writer.Close();
        SceneManager.LoadScene(combat);
    }

    public void QuitterAppli() {
        Application.Quit();
    }

    public void OuvrirChoix() {
        choixEquipe.SetActive(true);
        LoadPokemon();
    }

    public void RetourMenu() {
        boutonRouge.onClick.RemoveAllListeners();
        boutonBleu.onClick.RemoveAllListeners();
        choixEquipe.SetActive(false);
    }

    public void LoadPokemon(){

        for (int i = 0; i < PokemonsDispo.Count; i++)
        {
            string nomPokemon = PokemonsDispo[i];
            //Création d'un gameobject pour les Pokémons
            GameObject icon = new GameObject($"Bouton{nomPokemon}");
            //Propriétés de position :
            RectTransform trans = icon.AddComponent<RectTransform>();
            trans.transform.SetParent(emplacement.transform);
            trans.anchoredPosition = new Vector3(-250+i*100,-100,0);
            trans.localScale = new Vector3(1,1,1);
            //Image
            Image image = icon.AddComponent<Image>();
            var spritePokemon = Resources.Load<Sprite>($"Sprites/{PokemonsDispo[i]}/presentation");
            image.sprite = spritePokemon;
            //Bouton
            Button bouton = icon.AddComponent<Button>();
            ColorBlock cb = bouton.colors; //Couleurs du bouton
            cb.highlightedColor = new Color32(39,178,57,255);
            cb.selectedColor = new Color32(149,156,74,255);
            bouton.colors = cb;
            //Listener du bouton
            bouton.onClick.AddListener((delegate {SelectPokemon(nomPokemon);}));
            
        }
    }

    public void SelectPokemon(string nomPokemon){
        boutonRouge.onClick.RemoveAllListeners();
        boutonBleu.onClick.RemoveAllListeners();
        boutonRouge.onClick.AddListener((delegate {AddPokemonToTeam(nomPokemon,0);})); //Change le callback des boutons + pour avoir le bon Pokemon
        boutonBleu.onClick.AddListener((delegate {AddPokemonToTeam(nomPokemon,1);}));
    }

    public void AddPokemonToTeam(string nomPokemon, int team){
        boutonRouge.onClick.RemoveAllListeners(); //On supprime les anciens callback
        boutonBleu.onClick.RemoveAllListeners();
        GameObject icon = GameObject.Find($"Bouton{nomPokemon}");
        RectTransform trans = icon.GetComponent<RectTransform>();

        if (team == 0) {
            trans.transform.SetParent(titreRouge.transform);
            trans.anchoredPosition = new Vector3(0,-100-TeamRouge.Count*100,0);
            TeamRouge.Add(nomPokemon);
            rouge += "."+nomPokemon;
        } else {
            trans.transform.SetParent(titreBleu.transform);
            trans.anchoredPosition = new Vector3(0,-100-TeamBleu.Count*100,0);
            TeamBleu.Add(nomPokemon);
            bleu += "."+nomPokemon;
        }

        Destroy(icon.GetComponent<Button>());
    }
}
