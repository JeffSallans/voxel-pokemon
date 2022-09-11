using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

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

    protected List<IOpponentMove> moveUsedHistory = new List<IOpponentMove>();

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
        moveUsedHistory = new List<IOpponentMove>();
    }

    public async virtual Task opponentPlay()
    {
        // switch in the pokemon that is using the move
        if (nextOpponentMove.actingPokemon != battleGameBoard.opponentActivePokemon && nextOpponentMove.switchInOnUse)
        {
            battleGameBoard.switchOpponentPokemon(battleGameBoard.opponentActivePokemon, nextOpponentMove.actingPokemon);
            await battleGameBoard.worldDialog.ShowMessageAsync(nextOpponentMove.actingPokemon.pokemonName + " switched in");
        }
        lastMoveUsed = nextOpponentMove;
        moveUsedHistory.Insert(0, lastMoveUsed);
        if (nextOpponentMove.actingPokemon.isFainted)
        {
            var faintedMessage = nextOpponentMove.actingPokemon.pokemonName + " fainted before it could attack";
            await battleGameBoard.worldDialog.ShowMessageAsync(faintedMessage);
            return;
        }
        // Do no trigger if it was already played
        if (nextOpponentMove.playInstantly)
        {
            return;
        }

        nextOpponentMove.actingPokemon.hudAnimator.SetTrigger("onMoveHighlight");
        var messageList = nextOpponentMove.playMove();
        foreach (var message in messageList)
        {
            await battleGameBoard.worldDialog.ShowMessageAsync(message);
        }
    }

    public virtual string computeOpponentsNextMove()
    {
        // Randomly select move
        var moveIndex = Mathf.FloorToInt(Random.value * availableMoves.Count);
        nextOpponentMove = availableMoves[moveIndex];
        nextOpponentMove.onNextMoveSelect();
        // Trigger if it was already played
        if (nextOpponentMove.playInstantly)
        {
            // switch in the pokemon that is using the move
            if (nextOpponentMove.actingPokemon != battleGameBoard.opponentActivePokemon && nextOpponentMove.switchInOnUse)
            {
                battleGameBoard.switchOpponentPokemon(battleGameBoard.opponentActivePokemon, nextOpponentMove.actingPokemon);
            }
            return nextOpponentMove.playMove()[0];
        }
        return null;
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
