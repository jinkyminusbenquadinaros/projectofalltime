using System.Diagnostics;
using System.Media;
using System.Drawing;
using System.Windows.Forms;

namespace battleship_game
{
    public partial class Form1 : Form
    {
        List<Button> playerPositionButtons;
        List<Button> enemyPositionButtons;
        Random Random = new Random();
        private bool gameOver = false;
        int ships = 3;
        int round = 10;
        int playerScore;
        int enemyScore;

        public Form1()
        {
            InitializeComponent();
            Restartthedamngame();
        }

        private void EnemyPlayTimerEvent(object sender, EventArgs e)
        {


            if (enemyPositionButtons.Count > 0 && round > 0) 
            {
                round -= 1;
                txtRounds.Text = "round - " + round;

                int integer = Random.Next(playerPositionButtons.Count);
                // randomly selects something inside the player position buttons as the paremeter
                if ((string)playerPositionButtons[integer].Tag == "playerShip")
                {
                    SoundPlayer splayer = new SoundPlayer(Application.StartupPath + @"/kaboom.wav");
                    splayer.Play();
                    playerPositionButtons[integer].BackgroundImage = Properties.Resources.fireIcon;
                    // if they hit it shows a fire picture thats been imported this i stole off of google images when searchig cartoon fire
                    enemyMove.Text = playerPositionButtons[integer].Text;
                    playerPositionButtons[integer].Enabled = false;
                    // if it gets hit it gets disabled
                    playerPositionButtons[integer].BackColor = Color.Black;
                    playerPositionButtons.RemoveAt(integer);
                    //because its just randomly picking we dont want it to keep on picking the same thing so i stole this from stack overflow where it removes once its picked
                    enemyScore += 1;
                    txtEnemy.Text = enemyScore.ToString();
                    EnemyPlayTimer.Stop();
                }
                // just stops the enemy play timer pretty self explanatory
                else
                {
                    SoundPlayer splayer = new SoundPlayer(Application.StartupPath + @"/missingwhooshsound.wav");
                    splayer.Play();
                    // code below is just a copy of the code above but the backgroud image will be differet and score wont be added
                    playerPositionButtons[integer].BackgroundImage = Properties.Resources.missIcon;
                    // if they hit it shows a fire picture thats been imported this i stole off of google images when searchig cartoon fire
                    enemyMove.Text = playerPositionButtons[integer].Text;
                    playerPositionButtons[integer].Enabled = false;
                    // if it gets hit it gets disabled
                    playerPositionButtons[integer].BackColor = Color.Black;
                    playerPositionButtons.RemoveAt(integer);
                    //because its just randomly picking we dont want it to keep on picking the same thing so i stole this from stack overflow where it removes once its picked
                    EnemyPlayTimer.Stop();
                    // just stops the enemy play timer pretty self explanatory
                }

                if (round % 3 == 0)
                {
                    ShiftEnemyShips();
                    
                }

            }
            if (!gameOver && (round < 1 || enemyScore > 2 || playerScore > 2))
            {
                gameOver = true; 

                if (playerScore > enemyScore)
                {
                    SoundPlayer splayer = new SoundPlayer(Application.StartupPath + @"\winsoundforprogram.wav");
                    splayer.Play();
                    winningpicture.Visible = true;
                    MessageBox.Show("You win");
                    Restartthedamngame();
                }
                else if (enemyScore > playerScore)
                {
                    SoundPlayer splayer = new SoundPlayer(Application.StartupPath + @"\losingsoundforprogram.wav");
                    splayer.Play();
                    losingpicture.Visible = true;
                    MessageBox.Show("your ships have been sun Yikes");
                    Restartthedamngame();
                }
                else if (enemyScore == playerScore)
                {
                    MessageBox.Show("you drew with the ai");
                    Restartthedamngame();
                }
            }

        }
        private bool hasBattlefieldShifted = false; // track if the shift happened already because i dont want multiple it gets annoying

        private void ShiftEnemyShips()
        {
            if (hasBattlefieldShifted) return; // prevent multiple shifts

            // yakes 1 form player
            if (playerScore > 0) // make it so their score doesnt go negative
            {
                playerScore -= 1;
                txtPlayer.Text = playerScore.ToString();
            }

            // add extra round so you can have an extra turn
            round += 1;
            txtRounds.Text = "round - " + round;

            // below should cleart enemy grid
            foreach (Button button in enemyPositionButtons)
            {
                button.Tag = null; // removes tags
                button.BackgroundImage = null; // clears the iamges
                button.BackColor = Color.White; // Reset colour
            }

            // move ships randomly and make them flasg yellow is the plan for below 
            int shipsPlaced = 0;
            while (shipsPlaced < 3) 
            {
                int newPositionIndex = Random.Next(enemyPositionButtons.Count);
                if ((string)enemyPositionButtons[newPositionIndex].Tag == null) // checks if its empty or not to make it not repeat whats been done
                {
                    // flash the new pos with yellowness
                    enemyPositionButtons[newPositionIndex].BackColor = Color.Yellow;
                    Application.DoEvents(); 
                    System.Threading.Thread.Sleep(200); // updates ui rigt away and gives a small delay with yellow so you can actually see when it changes
                    enemyPositionButtons[newPositionIndex].BackColor = Color.White;

                    // assign ship to new location
                    enemyPositionButtons[newPositionIndex].Tag = "enemyShip";
                    Debug.WriteLine("Enemy new position: " + enemyPositionButtons[newPositionIndex].Text);

                    shipsPlaced++;
                }
            }

            // sets flag to make sure its seen that its shifted
            hasBattlefieldShifted = true;

            // Notify the player
            MessageBox.Show("The ocean has shifted, and enemy ships have moved! \n\nYou lost 1 point but gained an extra round.");
        }


        private void AttackButtonEvent(object sender, EventArgs e)
        {
            if (EnemyLocationListBox.Text != "")
            {
                var attackPosition = EnemyLocationListBox.Text.ToLower();
                // reason we put it to lower is because we want to compare this to the names of the buttons whiich are lower case
                int integer = enemyPositionButtons.FindIndex(a => a.Name == attackPosition);
                // integer going to store a number which is the number where attack position is at
                // the local variable stuff was gotten from stack overflow it just checks if the name of the variable is equal to attackposition
                // not fully sure what this does as im doing a final skim of this bit but it works so we ball

                if (enemyPositionButtons[integer].Enabled && round > 0)
                {
                    round -= 1;
                    // take one away as this round would have gone away
                    txtRounds.Text = "round - " + round;
                    // shows what the round is
                    if ((string)enemyPositionButtons[integer].Tag == "enemyShip")
                    {
                        SoundPlayer splayer = new SoundPlayer(Application.StartupPath + @"/kaboom.wav");
                        splayer.Play();
                        enemyPositionButtons[integer].Enabled = false;
                        // if the button is enemy ship it gets disabled
                        enemyPositionButtons[integer].BackgroundImage = Properties.Resources.fireIcon;
                        // shows that its been hit with the png thats been imported i tried to use picture box and do it in with files but that ended in tragedy so sameas 
                        //before where i serached up battle ship cartoon X
                        enemyPositionButtons[integer].BackColor = Color.Black;
                        playerScore += 1;
                        // just adds one to playerscore as they hit something
                        txtPlayer.Text = playerScore.ToString();
                        // makes the score the text
                        EnemyPlayTimer.Start();


                    }
                    else 
                    {
                        SoundPlayer splayer = new SoundPlayer(Application.StartupPath + @"/missingwhooshsound.wav");
                        splayer.Play();
                        enemyPositionButtons[integer].Enabled = false;
                        enemyPositionButtons[integer].BackgroundImage = Properties.Resources.missIcon;
                        // same as the fire but this time it didnt hit a ship so
                        enemyPositionButtons[integer].BackColor = Color.Black;
                        EnemyPlayTimer.Start();
                    }

                }
            }
            else
            {
                MessageBox.Show("Choose a location from the dropdown first", "IMPORTANT NOTICE");
                // doesnt let people attack without selecting something as that would probably break it or do something peculiar that im not trying t0 deal with 
                // this sucked to find out how to do when its so easy
            }
        }

        private void PlayerPositionButtonEvent(object sender, EventArgs e)
        {
            if (ships > 0)
            {
                var button = (Button)sender;
                // i crate a variable called button and then we identify whicever button was pressed as the sender event
                button.Enabled = false;
                button.Tag = "playerShip";
                button.BackColor = Color.Orange;
                // disabled the button and tagged it as player ship
                // the fact we have to spell colour as color is not cool at all
                ships -= 1;
                // you can see it just takes away one from the ship counter
            }
            if (ships == 0)
            {
                btnAttack.Enabled = true;
                btnAttack.BackColor = Color.Red;
                // just want a menacing colour that isnt orange so i picked red
                btnAttack.ForeColor = Color.White;

                txtHelp.Text = " now finally pick the attack position from the drop down menu and press attack";
            }



        }
        private void Restartthedamngame()
        {
            // planning to have near enough entire game in here so i have to keep track of the least amount of things will see how things go 
            playerPositionButtons = new List<Button>{w1, w2, w3, w4, x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4};
            enemyPositionButtons = new List<Button>{a1, a2, a3, a4, b1, b2, b3, b4, c1, c2, c3, c4, d1, d2, d3, d4};
            // each list holds the co ordinates of the grid
            EnemyLocationListBox.Items.Clear();
            winningpicture.Visible = false;
            losingpicture.Visible = false;
            // makes these 2 invisible til u win or lose

            // clears the list box everytime game restars so doesnt get clunky asf
            EnemyLocationListBox.Text = null;
            //we dont want any text to show up yet
            txtHelp.Text = "click on 3 buttons to start the game";
            for (int i = 0; i < enemyPositionButtons.Count; i++) //iterates through each button 
            {
                enemyPositionButtons[i].Enabled = true;
                enemyPositionButtons[i].Tag = null;
                //when enemy selects a location we link it to their ship with tag this needs to be nullified at the start
                enemyPositionButtons[i].BackColor = Color.White;
                //just sets the colour to white
                enemyPositionButtons[i].BackgroundImage = null;
                // theres going to be a flame png or x png when it either hits something or hits nish currently its neither so its null
                EnemyLocationListBox.Items.Add(enemyPositionButtons[i].Text);
                // adds to the list the text of the buttons its iterating through so each one is listed and can then be selected to attack :o
            }
            for (int i = 0; i < playerPositionButtons.Count; i++)
            {
                playerPositionButtons[i].Enabled = true;
                playerPositionButtons[i].Tag = null;
                playerPositionButtons[i] .BackColor = Color.White;
                playerPositionButtons[i].BackgroundImage = null;
                // same as before but this time we aint adding anything to the list
            }
            playerScore = 0;
            enemyScore = 0;
            round = 10;
            ships = 3;
            txtPlayer.Text = playerScore.ToString();
            txtEnemy.Text = enemyScore.ToString();
            // changing the text of the scores into their scores so you cna actually see how well youre doing 
            enemyMove.Text = "A1";
            btnAttack.Enabled = false;
            // dont want them to attack just yet
            enemyLocationPicker();
        }
        private void enemyLocationPicker()
        {
           
            
            for (int i = 0; i < 3; i++)
            {
                int integer = Random.Next(enemyPositionButtons.Count);
                if (enemyPositionButtons[integer].Enabled == true && (string)enemyPositionButtons[integer].Tag == null)
                {
                    enemyPositionButtons[integer].Tag = "enemyShip";
                    Debug.WriteLine("Enemy position: " + enemyPositionButtons[integer].Text);
                    // stole this from stack overflow in the debug menu it should show the 3 locations that it has picked so the person in control can see
                    // with this condition i can win whenever i want and hustle people as they dont realise i can see the positions in the bottom right if i so choose
                }
                else
                {
                    integer = Random.Next(enemyPositionButtons.Count);
                    // if we run into something that is enabled but it has a tag it redos this until it gets to 3
                }
            }
        }


    }
}
