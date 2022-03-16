using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IOpponentStrategy : MonoBehaviour
{
    /// <summary>
    /// The next move to display and execute on
    /// </summary>
    public IOpponentMove nextOpponentMove;

    /// <summary>
    /// The last move selected
    /// </summary>
    protected IOpponentMove lastMoveUsed;

    /// <summary>
    /// All the cards the last player played
    /// </summary>
    protected List<Card> lastPlayersTurn = new List<Card>();

    protected BattleGameBoard battleGameBoard;
    protected OpponentDeck opponentDeck
    {
        get { return battleGameBoard.opponent; }
    }

    /// <summary>
    /// All configured moves
    /// </summary>
    protected List<IOpponentMove> allMoves
    {
        get { return battleGameBoard.opponent.movesConfig; }
    }

    /// <summary>
    /// Moves available this turn
    /// </summary>
    protected List<IOpponentMove> availableMoves
    {
        get { return allMoves.Where(m => m.canUseMove && !m.actingPokemon.isFainted).ToList(); }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nextOpponentMove != null && nextOpponentMove.actingPokemon != null)
        {
            opponentDeck.party.ForEach(p => p.nextAttackText.text = "");
            nextOpponentMove.actingPokemon.nextAttackText.text = UnicodeUtil.replaceWithUnicode(nextOpponentMove?.moveDescription);
        }
    }

    public virtual void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;
    }

    public virtual string opponentPlay()
    {
        // switch in the pokemon that is using the move
        if (nextOpponentMove.actingPokemon != battleGameBoard.opponentActivePokemon)
        {
            battleGameBoard.switchOpponentPokemon(battleGameBoard.opponentActivePokemon, nextOpponentMove.actingPokemon);
        }
        lastMoveUsed = nextOpponentMove;
        if (nextOpponentMove.actingPokemon.isFainted)
        {
            return nextOpponentMove.actingPokemon.pokemonName + " fainted before it could attack";
        }
        return nextOpponentMove.playMove();
    }

    public virtual void computeOpponentsNextMove()
    {
        // Randomly select move
        var moveIndex = Mathf.FloorToInt(Random.value * availableMoves.Count);
        nextOpponentMove = availableMoves[moveIndex];
    }

    public virtual void onCardPlayed(Card move, Pokemon user, Pokemon target)
    {
        // Track the move the player made
        lastPlayersTurn.Add(move);
    }

    public virtual void onTurnEnd() {
        allMoves.ForEach(m => m.onTurnEnd());
    }

    public virtual void onOpponentTurnEnd() {
        // Reset player turn tracking on the start of their turn
        lastPlayersTurn.Clear();

        // Reset priority picks
        allMoves.ForEach(m => m.onOpponentTurnEnd());
    }

    public virtual void onBattleEnd() { }
}
