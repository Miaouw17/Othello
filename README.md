# Othello
Jeu de l'Othello en C# réalisé par Quentin Michel et Damian Petroff

## Présentation
Notre jeu d'Othello comprend toutes les mécaniques de jeu classique du jeu de l'Othello.
Il permet à l'utilisateur de jouer une partie contre un adversaire en local, de sauvegarder sa partie et de la recharger.

## Affichage graphique
Le GUI de notre Othello se sépare en 2 pages.
* Le menu
* Le plateau de Jeu

Le menu permet à l'utilisateur de lancer une nouvelle partie ainsi que de recharger une sauvegarde.

Le plateau de jeu comprend un jeu d'Othello en 9x7 comme demandé dans les contraintes du projet ainsi qu'un affichage des scores, des temps de réflexions de chaque joueur, un label d'indication du tour du joueur et deux boutons permettant de revenir en arrière dans les coups joué mais aussi de revenir à l'état initiale (Undo/Redo).

Les timers ainsi que les scores des joueurs sont bindé via le Binding de WPF.
NB : Pour binder les timers, nous avons dû créer un Converter permettant de formatter et de rendre un affichage de timer en minutes/secondes/millisecondes

Pour passer d'une page à l'autre, nous avons utilisé une Frame contenu dans la MainWindow dans laquelle nous allons passé les pages via la Navigation (this.NavigationService.Navigate(...))

## Algorithme de Jeu
Classe Joueur -> image...

Classe Board -> explication vague...

## Undo/Redo
Pour permettre à l'utilisateur d'utiliser les fonctionnalités d'Undo et Redo, nous avons mis en place 2 piles contenant des Tuple<int[], bool> qui contiennent l'etat du plateau de jeu (int[]) et le joueur à qui c'était de jouer (bool).

### Fonctionnement
À chaque coup joué par un joueur, juste avant d'entrer son coup dans le plateau de jeu, nous enregistrons l'etat du plateau dans la pile de Undo ainsi que le boolean permettant de définir le joueur qui joue.

Quand l'utilisateur utilise le bouton Undo, juste avant de réaliser la marche arrière, nous enregistrons l'état du plateau actuel dans la pile avec le boolean du joueur qui doit jouer afin de pouvoir revenir à l'état avant Undo.

Lorsqu'un joueur a jouer un coup après avoir Undo, la pile de Redo se vide pour éviter de pouvoir revenir dans une configuration étrange.

## Sauvegarde
Pour la sauvegarde, nous avons créer une classe à part du nom de "Save" contenant les informations nécessaire au chargement d'une partie. C'est à dire ;
* int[] values; // état du plateau
* bool isWhiteTurn; // tour du joueur
* long tsWhitePlayer; // timer joueur blanc
* long tsBlackPlayer; // timer joueur noir
* Stack<Tuple<int[], bool>> stackUndo; // pile de Undo
* Stack<Tuple<int[], bool>> stackRedo; // pile de Redo

Cette classe est donc sérializable via [Serializable] et sera un fichier contenant des informations en binaire.
Ce format a été choisi par défaut.
Lors du chargement d'une partie, nous allons déserializé cette cet objet "Save" pour en extraire les informations à replacer dans le plateau de jeu.
