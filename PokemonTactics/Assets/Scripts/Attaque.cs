using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Attaque : MonoBehaviour
{

    [SerializeField] public string Type;
    [SerializeField] public int Portee;
    [SerializeField] public int Rayon;
    [SerializeField] public int Forme;
    [SerializeField] public double Puissance;
    [SerializeField] public int Cout;

    public List<Vector3Int> FormeAttaque(Vector3Int ciblePosition, Vector3Int lanceurPosition, CommandeGrille cmd)
    {
        //Forme :
            // 0 --> carré
            // 1 --> Ligne droite en face
            // 2 --> Ligne de côté
            // On part du principe que la case de Rayon 1 est déjà colorée

        List<Vector3Int> zoneFrappe = new List<Vector3Int>();

        if (Forme == 0) { //On étend en carré depuis ciblePosition
            if (Rayon == 2) {
                zoneFrappe.Add(new Vector3Int(ciblePosition.x+1, ciblePosition.y, ciblePosition.z));
                zoneFrappe.Add(new Vector3Int(ciblePosition.x-1, ciblePosition.y, ciblePosition.z));
                zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y+1, ciblePosition.z));
                zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y-1, ciblePosition.z));
            }
        }
        else if (Forme == 1) { //On étend en ligne droite selon où est placé la cible et le lanceur (portée forcément de 1)
            if (lanceurPosition.y > ciblePosition.y) { //Le sort part vers le haut
                for (int i = 1; i < Rayon; i++){
                    Vector3Int cell = new Vector3Int(ciblePosition.x, ciblePosition.y-i, ciblePosition.z);
                    if (cmd.grilleTactique.GetTile<Tile>(cell).name.ToString()!="CaseTactique") //L'attaque s'arrête si elle croise un mur
                        break;
                    else
                    zoneFrappe.Add(cell);
                }
            } else if (lanceurPosition.y < ciblePosition.y) { //Le sort part vers le bas
                for (int i = 1; i < Rayon; i++){
                    Vector3Int cell = new Vector3Int(ciblePosition.x, ciblePosition.y+i, ciblePosition.z);
                    if (cmd.grilleTactique.GetTile<Tile>(cell).name.ToString()!="CaseTactique")
                        break;
                    else
                    zoneFrappe.Add(cell);
                }
            } else if (lanceurPosition.x > ciblePosition.x) { //Le sort part vers la gauche
                for (int i = 1; i < Rayon; i++){
                    Vector3Int cell = new Vector3Int(ciblePosition.x-i, ciblePosition.y, ciblePosition.z);
                    if (cmd.grilleTactique.GetTile<Tile>(cell).name.ToString()!="CaseTactique")
                        break;
                    else
                    zoneFrappe.Add(cell);
                }
            } else { //le sort part vers la droite
                for (int i = 1; i < Rayon; i++){
                    Vector3Int cell = new Vector3Int(ciblePosition.x+i, ciblePosition.y, ciblePosition.z);
                    if (cmd.grilleTactique.GetTile<Tile>(cell).name.ToString()!="CaseTactique")
                        break;
                    else
                    zoneFrappe.Add(cell);
                }
            }
        }
        else if (Forme == 2) { //3 cas : gauche/droite, haut/bas, diagonale
            //On calcule l'écart entre le lanceur et la cible, sur deux axes
            int ecartX = Mathf.Abs(ciblePosition.x - lanceurPosition.x);
            int ecartY = Mathf.Abs(ciblePosition.y - lanceurPosition.y);

            if (ecartX > ecartY) {
                for (int i = 1; i < Rayon; i++) {
                    zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y-i, ciblePosition.z));
                    zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y+i, ciblePosition.z));
                }
            } else if (ecartY > ecartX) {
                for (int i = 1; i < Rayon; i++) {
                    zoneFrappe.Add(new Vector3Int(ciblePosition.x-i, ciblePosition.y, ciblePosition.z));
                    zoneFrappe.Add(new Vector3Int(ciblePosition.x+i, ciblePosition.y, ciblePosition.z));
                }
            } else { //4 cas de diagonales
                if (lanceurPosition.y > ciblePosition.y && lanceurPosition.x > ciblePosition.x) { //Bas Gauche
                    for (int i = 1; i < Rayon; i++) {
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x-i, ciblePosition.y, ciblePosition.z));
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y-i, ciblePosition.z));
                    }
                }
                if (lanceurPosition.y > ciblePosition.y && lanceurPosition.x < ciblePosition.x) { //Bas Droite
                    for (int i = 1; i < Rayon; i++) {
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x+i, ciblePosition.y, ciblePosition.z));
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y-i, ciblePosition.z));
                    }
                }
                if (lanceurPosition.y < ciblePosition.y && lanceurPosition.x > ciblePosition.x) { //Haut Gauche
                    for (int i = 1; i < Rayon; i++) {
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x-i, ciblePosition.y, ciblePosition.z));
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y+i, ciblePosition.z));
                    }
                }
                if (lanceurPosition.y < ciblePosition.y && lanceurPosition.x < ciblePosition.x) { //Haut Droite
                    for (int i = 1; i < Rayon; i++) {
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x+i, ciblePosition.y, ciblePosition.z));
                        zoneFrappe.Add(new Vector3Int(ciblePosition.x, ciblePosition.y+i, ciblePosition.z));
                    }
                }
            }
        }

        return zoneFrappe;
    }
}
