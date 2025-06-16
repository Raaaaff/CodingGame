# Jeu de Morpion Multijoueur - C# Console

## Description

Ce projet est un jeu de morpion multijoueur en ligne (tour par tour) codé en C# avec sockets TCP, sans framework.  
Le serveur gère l’état du jeu, les règles, la synchronisation et les clients.

## Fonctionnalités

- 2 joueurs humains en ligne
- Affichage en console du plateau actualisé
- Validation des coups (aucune triche possible)
- Synchronisation temps réel
- Détection de victoire ou match nul

---

## 📦 Contenu

- `MorpionServer.exe` – le serveur
- `MorpionClient.exe` – le client (à lancer deux fois)
- Code source complet en C#

---

## 🧪 Instructions d'utilisation

1. **Lancer le serveur**
   ```bash
   ./MorpionServer.exe
