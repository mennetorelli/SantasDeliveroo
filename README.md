# Santa's Deliveroo

## Utilizzo

### Camera

- **Spacebar**: passaggio tra modalità free e tactical

#### Free mode

- **WASD**: movimento camera
- **QE**: regolazione altezza camera
- **Right click + mouse movement**: rotazione camera

#### Tactical mode

- **AD**: rotazione camera attorno all'asse y globale
- **WS**: regolazione altezza camera

### Selezione/azione

- **Left click**: selezione oggetto

#### Azioni di Babbo Natale

- **Shift**: cambia modalità della camera

**When collect/deliver action mode enabled:**

- **Right click**: esegue una azione collect o deliver
  **When move action modeis enabled:**
- **Mouse scroll:** cambia l'offset della destinazione rispetto all'asse y
- **Right click**: esegue un'azione move

### Altri comandi

- **I**: abilita/disabilita il pannello che descrive i comandi
- **Alt**: mette in pausa

# Funzionamento dell'applicazione

## Elementi di gioco

I quattro principali elementi in gioco (Babbi Natale, Befane, regali e case)
estendono una classe astratta chiamata `SelectableComponentBase`.
Come dice il nome, questa classe consente agli elementi di essere selezionati dall'utente
(con il tasto destro del mouse).
`SelectableComponentBase` fornisce tre metodi virtuali/astratti:

- `OnSelected()` viene invocato quando l'oggetto è stato selezionato, serve per mostrare del feedback all'utente.
  Ogni implementazione della classe mostra il feedback in un modo specifico:
  per esempio quando si seleziona un Babbo Natale vengono evidenziati i waypoint sulla mappa
  e le case di cui ha raccolto dei regali.
- `OnSelected()` viene invocato quando l'oggetto è stato deselezionato,
  e viceversa serve per smettere di mostrare del feedback visivo.
- `FormatDetails()` serve per formattare i dettagli dell'oggetto selezionato,
  in modo tale che siano visibili nel pannello in basso a destra.

Gli oggetti interagiscono tra di loro normalmente tramite dei `Collider` e il metodo `OnTriggerEnter()`.
Una volta che è stato invocato il metodo `OnTriggerEnter()`,
l'oggetto in questione aggiorna il proprio stato e quello dell'oggetto che ha toccato,
e notifica il `GameManager` e gli elementi della UI.

## Input Manager

Per realizzare il progetto è stato utilizzato il nuovo Input System di Unity.
Le classi che gestiscono l'input dell'utente sono `InputManager.cs`
e i controller delle due camere.
`InputManager.cs` gestisce tutta l'interazione necessaria per selezionare oggetti
e svolgere azioni con i Babbi Natale.
`InputManager` mantiene un riferimento dell'elemento selezionato,
in modo tale da controllare, per esempio, se è stato selezionato correttamente un Babbo Natale
prima di eseguire un'azione.
I controller delle due camere gestiscono la loro posizione/rotazione
all'interno dell'area di gioco.

## UI

Nella schermata di gioco ci sono tre pannelli, ognuno gestito dal corrispondente script.
Questi oggetti sono dei singleton, perchè è necessario
che tutti gli elementi in gioco possano accedere al loro reference.

- In alto a sinistra è presente il pannello che contiene le informazioni generali del gioco,
  come tempo, numero di regali da consegnare, ecc.
- In alto centrato c'è il pannello che mostra i messaggi che arrivano dagli elementi del gioco.
- In basso a destra c'è il pannello con i dettagli dell'elemento selezionato.
  Ad esempio, se si seleziona una casa, il pannello visualizza
  le informazioni sulla casa e sui regali che deve ricevere.

## Livelli

Nel gioco ci sono 10 livelli di difficoltà crescente.
Ogni configurazione di un livello corrisponde a un asset generato da uno `ScriptableObject`.
La scena di gioco viene generata proceduralmente dall'oggetto `GameManager`
a seconda della configurazione del livello.

Ci sono diverse aree sulla mappa entro le quali vengono generati casualmente gli oggetti:
le Befane vengono generate nella loro apposita area, così come i Babbi Natale,
mentre i regali sono generati casualmente su tutta l'area di gioco.

In ogni configurazione è possibile impostare diversi parametri, come:

- tempo massimo per completare il livello
- numero di regali che vengono generati
- numero di case che vengono attivate
- numero di Babbi Natale
- numero di Befane
- velocità dei Babbi Natale
- velocità delle Befane
- raggio di azione delle befane
- durata delle azioni delle befane (cioè quando una Befana non insegue un Babbo Natale,
  questa si muove in modo random sulla mappa, e ogni tot secondi cambia direzione.)

Chiaramente, il settaggio di questi parametri influenza la difficoltà del livello: ad esempio,
più Befane sono presenti e più si muovono velocemente e a zig-zag,
maggiore sarà la difficoltà per i Babbi Natale di evitarle.

La mappa di gioco è unica, e corrisponde alla stessa scena che viene caricata per tutti i livelli.
Le case sono già presenti sulla mappa all'avvio del livello.
All'avvio del livello il `GameManager` seleziona delle case in modo casuale
(a seconda del numero specificato nella configurazione);
queste case vengono "attivate" e colorate per evidenziarle rispetto alle altre.

# Scelte implementative

## Gestione della camera

Il gioco prevede due modalità, una chiamata free view e una tactical view.

Sebbene nelle specifiche sia indicato solo che nella tactical view
si possano eseguire azioni e selezionare oggetti,
è stata lasciata la possibilità di selezionare gli oggetti anche in free view,
tuttavia non di svolgere le azioni tramite i Babbi Natale.

Nella tactical mode, tuttavia, è stata prevista la possibilità di due ulteriori movimenti
rispetto a quanto indicato nelle specifiche:
la rotazione attorno all'asse y globale, in modo tale da poter vedere la mappa da diverse angolazioni.
Inoltre, con i tasti QE si può regolare l'altezza della camera.
Mentre l'altezza della camera varia, essa è sempre orientata verso il centro della mappa,
così che si ha una visione chiara della mappa indipendentemente dalla sua posizione.

## Azioni di Babbo Natale

Quando si seleziona un Babbo Natale, si possono eseguire due possibili tipi di azioni: move oppure collect/deliver.
La differenza principale sta nel fatto che mentre nella prima azione l'utente può cliccare col pulsante sinistro
in un punto qualsiasi della mappa di gioco,
per le azioni collect/deliver è necessario selezionare col pulsante sinistro un regalo o una casa.
Si è dovuto quindi trovare un modo per evitare la sovrapposizione di queste azioni
e allo stesso tempo rendere fluido il passaggio da un'azione all'altra.
Premendo il tasto Shift si abilita (disabilita) la modalità move,
e un piano semitrasparente compare all'altezza del Babbo Natale selezionato
per semplificare la selezione di un punto nell'area di gioco.
Sul piano è disegnata una linea che va dal Babbo Natale selezionato
a un punto del piano a seconda della posizione del mouse, e che segue il movimento del mouse stesso.
In questo modo si può controllare il movimento del Babbo Natale su un piano xz.
Se l'utente utilizza la rotellina del mouse, il punto di arrivo si alza/abbassa di un certo offset dal piano,
in modo tale da poter muovere il Babbo Natale anche nella dimensione data dall'asse y.
Cliccando con il pulsante sinistro, il Babbo Natale comincia a muoversi nel punto stabilito.
Da notare che il Babbo Natale non può cliccare su un punto che supera coordinate dell'area di gioco.
Questo è stato ottenuto facendo in modo che, quando la modalità move è attiva,
il Raycast generato dal mouse può intercettare solo il layer a cui il piano semitrasparente è stato assegnato.

Le specifiche parlano di una lista di waypoint che possono essere aggiunti come sequenza di azioni di un Babbo Natale
cliccando col pulsante destro mentre si tiene premuto il tasto ctrl.
Siccome è specificato che:

- se non viene premuto il pulsante ctrl
  la lita di azioni **pianificate** viene sovrascritta dall'azione corrente,
- quando si ordina a Babbo Natale di eseguire un'azione essa viene eseguita in real time

Si è deciso per questi due motivi che l'azione che è in esecuzione
non viene mai sovrascritta ma deve essere portata a termine.
In altre parole, non c'è modo di annullare un'azione in esecuzione da Babbo Natale eseguendo una nuova azione,
ma la nuova azione viene eseguita al completamento della prima.
