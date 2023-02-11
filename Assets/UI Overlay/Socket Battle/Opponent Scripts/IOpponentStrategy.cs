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
    public OpponentCardMove nextOpponentMove;

    /// <summary>
    /// The last move selected
    /// </summary>
    protected OpponentCardMove lastMoveUsed;

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
    /// The cards to be drawn
    /// </summary>
    public List<OpponentCardMove> deck;

    /// <summary>
    /// The cards available to be played
    /// </summary>
    public List<OpponentCardMove> hand;

    /// <summary>
    /// The cards that have been played
    /// </summary>
    protected List<OpponentCardMove> discard;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (battleGameBoard != null && battleGameBoard.showOppAttack && nextOpponentMove != null && nextOpponentMove.card.owner != null)
        {
            opponentDeck.party.ForEach(p => p.nextAttackText.text = "");
            nextOpponentMove.card.owner.nextAttackText.text = UnicodeUtil.replaceWithUnicode(nextOpponentMove.card.cardName);
        }
    }

    /// <summary>
    /// Initialize the deck, hand, and discard like the player does for themselves in BattleGameBoard
    /// </summary>
    /// <param name="_battleGameBoard"></param>
    public virtual void onBattleStart(BattleGameBoard _battleGameBoard)
    {
        battleGameBoard = _battleGameBoard;

        // Init deck
        deck = opponentDeck.initDeck.Select(card =>
        {
            card.cardInteractEnabled = false;

            var opponentMove = new OpponentCardMove();
            opponentMove.card = card;
            opponentMove.turnPriority = 3;
            opponentMove.selectedTargetPokemon = null;
            return opponentMove;
        }).ToList();

        // Shuffle deck
        Shuffle(deck);

        // Move deck into the correct position
        deck.ForEach(move =>
        {
            move.card.Translate(battleGameBoard.oppDeckLocation.transform.position, "OpponentStrategyOnBattleStart");
        });

        // Init empty discard
        discard = new List<OpponentCardMove>();

        // Init hand and draw
        hand = new List<OpponentCardMove>();
        for (var i = 0; i < battleGameBoard.handSize; i++)
        {
            drawCard();
        }

        // Add energy to random target
        var energyStatRandomTarget = getPokemonToAddEnergyTo();
        addEnergy(energyStatRandomTarget);
    }

    /// <summary>
    /// Draws a card from deck to hand. Has checks to prevent drawing too many cards or the deck being empty before drawing a card.
    /// </summary>
    protected virtual void drawCard()
    {
        // Check if hand is full
        if (hand.Count == battleGameBoard.maxHandSize) return;

        // Check if reshuffle is needed
        if (deck.Count <= 0)
        {
            deck.AddRange(discard);
            Shuffle(deck);
            discard.RemoveAll(c => true);

            // Remove all deck cards with fainted pokemon
            for (int i = deck.Count - 1; i >= 0; i--)
            {
                if (deck[i].card.owner.isFainted) cardDiscard(deck[i], deck[i].card.owner, false, true);
            }
        }

        // Draw card
        var cardDrawn = deck.First();
        hand.Add(cardDrawn);
        deck.Remove(cardDrawn);
        cardDrawn.card.onDraw();
    }

    public virtual string computeOpponentsNextMove()
    {
        // Draw a card to hand
        drawCard();

        // Compute targets for all cards
        hand.ForEach(move => move.selectedTargetPokemon = getSelectedTarget(move, battleGameBoard));

        // Find all hand cards you can play
        var playableMoves = hand.Where(move => move.canUseMove).ToList();

        // Calculate priority score
        playableMoves.ForEach(move => move.turnPriority = getPriorityScore(move, battleGameBoard));

        // Select top card
        nextOpponentMove = playableMoves.OrderByDescending(m => m.turnPriority).FirstOrDefault();

        return null;
    }

    /// <summary>
    /// Return the random selected target for.
    /// </summary>
    /// <param name="move">Modifies move.selectedTargetPokemon to the result</param>
    /// <param name="battleGameBoard"></param>
    /// <returns></returns>
    protected virtual Pokemon getSelectedTarget(OpponentCardMove move, BattleGameBoard battleGameBoard)
    {

        // Get potential targets
        var potentialTargets = battleGameBoard.allPokemon.Where(poke => move.card.canTarget(poke)).ToList();

        // Init best results
        var bestTargetSoFar = potentialTargets.FirstOrDefault();
        var bestScoreSoFar = 0f;

        // Compute score for each target
        potentialTargets.ForEach(target =>
        {
            move.selectedTargetPokemon = target;
            var score = getPriorityScore(move, battleGameBoard, false);

            if (score > bestScoreSoFar)
            {
                bestScoreSoFar = score;
                bestTargetSoFar = target;
            }

        });

        // Return the target with the best score
        move.selectedTargetPokemon = bestTargetSoFar;
        return bestTargetSoFar;
    }

    /// <summary>
    /// Return a number to determine how useful it is to play the given move
    /// </summary>
    /// <param name="move"></param>
    /// <param name="battleGameBoard"></param>
    /// <returns></returns>
    protected virtual float getPriorityScore(OpponentCardMove move, BattleGameBoard battleGameBoard, bool includedRandomModifier = true)
    {
        var allTargets = move.card.getTarget(move.card.targetType, move.selectedTargetPokemon);

        // Initialize priority score to 5 for cards
        var turnPriority = 5f;

        // Score Mod: +3 if the card is super effective against the target
        var superEffectiveAgainstTarget = allTargets.Any(target => TypeChart.getEffectiveness(move.card, target) > 1);
        if (superEffectiveAgainstTarget) turnPriority += 3;

        // Score Mod: +1 x energy count
        turnPriority += move.card.prefabCost.Count;

        // Score Mod: +1 if the card has atk, def, or special modifiers and the target is 0
        var targetOfStatChange = (move.card.statusAffectsUser) ? new List<Pokemon>() { move.card.owner } : allTargets;
        var targetIsMissingStatCardHas = targetOfStatChange.Any(target =>
        {
            var hasStatNotInitializedYet = move.card.attackStat > 0 && target.attackStat == 0 ||
                move.card.defenseStat > 0 && target.defenseStat == 0 ||
                move.card.specialStat > 0 && target.specialStat == 0;
            return hasStatNotInitializedYet;
        });
        if (targetIsMissingStatCardHas) turnPriority += 1;

        // Score Mod: +0-2 random value
        if (includedRandomModifier) turnPriority += Random.Range(0f, 2f);

        return turnPriority;
    }

    /// <summary>
    /// Returns the best pokemon to attach an energy to
    /// </summary>
    /// <returns></returns>
    protected virtual Pokemon getPokemonToAddEnergyTo()
    {
        // Pick a random pokemon to add an energy to
        var aliveParty = battleGameBoard.opponent.party.Where(p => !p.isFainted).ToList();
        var pokemonToAddEnergyTo = aliveParty[UnityEngine.Random.Range(0, aliveParty.Count)];

        // Find all hand cards you can't play
        var movesThatCantBePlayed = hand.Where(move => !move.canUseMove && !move.card.owner.isFainted && move.card.owner.attachedEnergy.Count < move.card.owner.maxNumberOfAttachedEnergy).ToList();

        // Select a card at random and add energy to owner
        if (movesThatCantBePlayed.Count > 0)
        {
            var cardIndex = UnityEngine.Random.Range(0, movesThatCantBePlayed.Count); // This line could be based on priority score instead of being random
            pokemonToAddEnergyTo = movesThatCantBePlayed[cardIndex].card.owner;
        }
        
        return pokemonToAddEnergyTo;
    }

    /// <summary>
    /// Call to play the opponents selected card
    /// </summary>
    /// <returns></returns>
    public async virtual Task opponentPlay()
    {
        // Check that move can still be used after player's turn
        if (nextOpponentMove == null || !nextOpponentMove.canUseMove)
        {
            computeOpponentsNextMove();
        }

        // Check the opponent's turn should be skipped because they don't have a card to play
        if (nextOpponentMove == null)
        {
            await battleGameBoard.worldDialog.ShowMessageAsync(battleGameBoard.opponentActivePokemon.pokemonName + " is waiting for a command.");

            // Add energy to random target
            var energyStatTarget = getPokemonToAddEnergyTo();
            addEnergy(energyStatTarget);
            await battleGameBoard.worldDialog.ShowMessageAsync("Added energy to " + energyStatTarget.pokemonName);

            return;
        }

        // Switch in the pokemon that is using the move
        if (nextOpponentMove.card.owner != battleGameBoard.opponentActivePokemon && nextOpponentMove.card.switchInOnUse)
        {
            battleGameBoard.switchPokemon(battleGameBoard.opponentActivePokemon, nextOpponentMove.card.owner);
            await battleGameBoard.worldDialog.ShowMessageAsync(nextOpponentMove.card.owner.pokemonName + " switched in.");
        }

        // Check if all targets are fainted
        if (nextOpponentMove.card.owner.isFainted)
        {
            var faintedMessage = nextOpponentMove.card.owner.pokemonName + " fainted before it could attack.";
            await battleGameBoard.worldDialog.ShowMessageAsync(faintedMessage);
            return;
        }

        // Annouce and show the card
        nextOpponentMove.card.Translate(battleGameBoard.oppPlayedCardLocation.transform.position, "opponentPlay1");
        await battleGameBoard.worldDialog.ShowMessageAsync(nextOpponentMove.card.owner.pokemonName + " used " + nextOpponentMove.card.cardName);
        nextOpponentMove.card.Translate(battleGameBoard.oppDeckLocation.transform.position, "opponentPlay2");

        // Play the card
        hand.Remove(nextOpponentMove);
        nextOpponentMove.card.owner.hudAnimator.SetTrigger("onMoveHighlight");
        nextOpponentMove.card.play(nextOpponentMove.card.owner, nextOpponentMove.selectedTargetPokemon);

        // discard 1 energy to play
        if (!nextOpponentMove.card.keepEnergiesOnPlay)
        {
            nextOpponentMove.card.owner.DiscardEnergy(nextOpponentMove.card.owner.attachedEnergy[0]);
            //await battleGameBoard.worldDialog.ShowMessageAsync("Energy discared to play " + nextOpponentMove.card.cardName);
        }

        // Track the move and move to discard
        lastMoveUsed = nextOpponentMove;
        cardDiscard(nextOpponentMove, nextOpponentMove.card.owner, true);

        // Add energy to random target
        var energyStatRandomTarget = getPokemonToAddEnergyTo();
        addEnergy(energyStatRandomTarget);
        await battleGameBoard.worldDialog.ShowMessageAsync("Added energy to " + energyStatRandomTarget.pokemonName);
    }

    /// <summary>
    /// Discards the given card
    /// </summary>
    /// <param name="target">The move to discard</param>
    /// <param name="user">The pokemon that used the move</param>
    /// <param name="wasPlayed">True if the discard was after playing</param>
    public void cardDiscard(OpponentCardMove target, Pokemon user, bool wasPlayed, bool deckDiscard = false)
    {
        // Keep track of card in discard if it is not single use
        if (!(wasPlayed && target.card.isSingleUse))
        {
            discard.Add(target);
        }


        if (deckDiscard)
        {
            deck.Remove(target);
        }
        else
        {
            hand.Remove(target);
        }

        battleGameBoard.cardEventService.onDiscard(target.card, wasPlayed);
    }

    /// <summary>
    /// Add a normal energy prefab to the given target
    /// </summary>
    /// <param name="target"></param>
    private void addEnergy(Pokemon target)
    {
        // Don't add energy if we are already maxed
        if (target.maxNumberOfAttachedEnergy == target.attachedEnergy.Count) return;

        var energyParent = target.gameObject;
        var energy = Instantiate(battleGameBoard.opponent.normalEnergyPrefab, energyParent.transform);
        energy.transform.position = battleGameBoard.energyDeckLocation.transform.position;
        target.attachedEnergy.Add(energy.GetComponent<Energy>());
    }

    public virtual void onCardPlayed(Card move, Pokemon user, Pokemon target)
    {
        // Track the move the player made
        lastPlayersTurn.Add(move);
    }

    public virtual void onTurnEnd() {
        // don't do card stuff here
    }

    public virtual void onOpponentTurnEnd() {
        // Reset player turn tracking on the start of their turn
        lastPlayersTurn.Clear();

        // don't do card stuff here
    }

    public virtual void onBattleEnd() { }

    /// <summary>
    /// Fisher-Yates Shuffle on the list
    /// https://stackoverflow.com/a/1262619
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    protected void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Mathf.FloorToInt(Random.value * n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
