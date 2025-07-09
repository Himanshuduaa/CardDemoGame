GameManager.cs	
Main controller for creating the card grid, handling card selection, checking matches, and managing game logic

Card.cs	
Represents individual card behavior (flip animation, match checking, state reset)

GameStateManager.cs	
Manages saving and loading game state data via JSON files

ShuffleCards.cs	
Static extension class to shuffle card

========================== Features =========================
Card flipping animations using DOTween
Save & Load functionality for game state
Match detection with paired cards
=============================================================

Design Patterns Used :-
1.While not a strict Singleton, GameStateManager acts like a global controller component, accessible from GameManager.
2.Each card encapsulates its own actions (flip, match, reset) inside the Card class, which can be considered a form of Command pattern since each card knows how to execute operations on itself.
3.Each card listens for click events via Button.onClick.AddListener(), allowing decoupled event-driven behavior.

=========Dependencies========
DOTween for smooth animations
