using UnityEngine;
using System.Collections;

public class TypingScript : MonoBehaviour {

	/* Speed Typer
	The game that is all about typing, typing at speed...

	Instructions
	Click a challenge, press play and type the letter that comes up.
	Web only game
	*/

	/* Improvements
	Main page:
	Resize challenge text font.
	Show best time in description/letters info box?
	Fix size of description/letters info box.
	Change "Count to 10" letters to start from 1 and end at 10, same with 20.
	Keep button selected when pressed for challege.
	With random challenges show all possible or just current?

	Game page:
	Resize "Type:" font.
	Resize "Finished:" font.
	On survival show a timer of two seconds or something that resets on each key press.
	Penalty for typing the wrong letter on all modes.
	Show a picture of a keyboard and the key to press.
	
	Options page:
	Adjust how the A- and A+ change font/GUI size.
	Resize challenge text font.
	Enable "Reset score" to work.
	Change "Reset all scores" confirmation button.
	Reset all preferences on computer after fixing them, then make sure all scores save.

	About page:
	Text doesn't fit.
	Change info/text.
	*/

	
	/* Challenges.
	Each challenge has a name and description.
	Challenge letters are the letters the user has to type.
	Challenges 6, 7 and 8 use string arrays as they contain multiple words.
	*/
	int challenges = 8;

	string challenge1Name = "The ABC's";
	string challenge1Description = "Type the alphabet in chronological order";
	string challenge1Letters = "abcdefghijklmnopqrstuvwxyz";

	string challenge2Name = "ABC's gone wrong";
	string challenge2Description = "Alphabet backwards";
	string challenge2Letters = "zyxwvutsrqponmlkjihgfedcba";

	string challenge3Name = "Brown fox";
	string challenge3Description = "Classic typing exercise";
	string challenge3Letters = "thequickbrownfoxjumpsoverthelazydog";

	string challenge4Name = "Count to 10";
	string challenge4Description = "0-9";
	string challenge4Letters = "0123456789";

	string challenge5Name = "Count to 20";
	string challenge5Description = "0-20";
	string challenge5Letters = "01234567891011121314151617181920";

	string challenge6Name = "Random names";
	string challenge6Description = "A random persons name";
	string[] challenge6Letters = {"steve", "alan", "john", "edward", "kevin", "ken"};

	string challenge7Name = "Random letters";
	string challenge7Description = "Random sequence of letters";
	string[] challenge7Letters = {"asdanoin", "ownikibqw", "oluijni", "aaikiisd", "iwnuiwrg", "jhni"};

	string challenge8Name = "Double the number";
	string challenge8Description = "Starting at 1, double it and so on";
	string[] challenge8Letters = {"1", "2", "4", "8", "16", "32", "64", "128", "256", 
								  "512", "1024", "2048", "4096", "8192", "16384", "32768", 
								  "65536", "131072", "262144", "524288", "1048576"};
	int challenge8Pos;


	// Selected challenge name, description and letters.
	string challengeName;
	string challengeDescription;
	string challengeLetters;
	int challengeSelected; // When a challenge is selected, highlight.

	int challengePage = 1;
	int challengePageTotal = 2;
	string scoresKey; // playerprefs key
	bool resetAllScores;
	string customName = "customName";

	int letterNum = 0; // Letter currently on in the challenge string.
	char letter; // The current letter the player needs to type.
	float time;

	// Survival options (need to optimise)
	bool showSurvivalOptions = false;
	float survivalTime;
	float survivalTimeStart = 1;
	float survivalAdd;
	float survivalAddStart = 1;
	float survivalAddLowLimit = 0.2f;
	float survivalAddToMinus = 0.1f;


	int gameState;
	enum GameStateEnum
    {
        MENU = 0, // Menu page
		GAME, // Game page
		GAMEPLAYING, // Game is running
		GAMEFINISHED, // Game is finished
        OPTIONS, // Options page
        ABOUT // About page
    };
	bool isRunning; // Game is in progress
	bool isFinished = true; // Game is finished

	bool isSurvivalMode;


	public GUISkin myGUISkin;


	int scrolled;
	int scrollMin = 1;
	int scrollMax = 3; //15

	string aboutInfo = "Speed Typer is based on the Finger Frenzy flash" +
						"game and was created using Unity 3D" +
						"\n\nDesigner: Ian" +
						"\nProgrammer: Ian";

	// Use this for initialization
	void Start () {
		gameState = (int)GameStateEnum.MENU;
		setChallenge("1");

		// check screen size, change scrolled. 1050, 1680 = 3. 1920, 1080 = 6
		//scrolled = Screen.height / 200;
		scrolled = 2;

		// Show options at start if player has never played.
		/*
		for(int i=1; i<challenges; i++){
			if(PlayerPrefs.HasKey("challenge" + i)){
				// player has played before.
				isOptionsScreen = false;
			}
		}
		*/
		reset();
	}

	// Update is called once per frame
	void Update () {
		// add to time
		if(isRunning){
			time += Time.deltaTime;
			if(isSurvivalMode && survivalTime > 0){
				survivalTime -= Time.deltaTime;
			}
			else if (isSurvivalMode && survivalTime <= 0){
				isRunning = false;
				isFinished = true;
			}
		}
		// Press currect letter
		if(!isFinished){
			if(Input.GetKeyDown(letter.ToString())){
				isRunning = true;
	
				if(!isSurvivalMode){
					letterNum++;
		
					// Not finished challenge
					if(letterNum != challengeLetters.Length){
							// Timer continues if user presses last key a second time
					// Fixed with this if else
						if(letterNum < challengeLetters.Length){
							letter = challengeLetters[letterNum];
						}
						else{
							isRunning = false;
							isFinished = true;
						}
					}
					// end of challenge
					else{
						// When challenge 8, go to next word, do not finish
						if(challengeSelected == 8 && 
						   challengeLetters != challenge8Letters[challenge8Letters.Length-1]){
							// Check current position in array for challenge 8, go to next
							//challenge8Pos = challengeLetters.IndexOf(challengeLetters); 
							challenge8Pos++;
							/*print ("Challenge8Pos: " + (challenge8Pos-1) + 
							       ", Letter before: " + challengeLetters +
							       ", Letter after: " + challenge8Letters[challenge8Pos]);*/

							challengeLetters = challenge8Letters[challenge8Pos];
							string myString = challenge8Letters[challenge8Pos];
							letter = myString[0];
							letterNum = 0;
						}
						else{
							isRunning = false;
							isFinished = true;
						}
						/*
						isRunning = false;
						isFinished = true;*/
					}
				}
				// Survival
				else{
					// Random letter
					letter = challenge1Letters[Random.Range(0, challenge1Letters.Length-1)];
					survivalTime += survivalAdd;
					if(survivalAdd > survivalAddLowLimit){
						survivalAdd -= survivalAddToMinus;
					}
				}
			}
		}

		// Space to reset game
		if(Input.GetKeyDown(KeyCode.Space)){
			reset();
		}

		// Escape/Back button to quit
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}

















	Rect backButtonRect = new Rect (150, 0, 120, 25);
	Rect scalingButtonsRect = new Rect (465, 0, 150, 35);

	Rect menuChallengeRect = new Rect (150, 80, 250, 530);
	Rect menuButtonsRect = new Rect (415, 80, 200, 430);

	Rect gameChallengeRect = new Rect (150, 80, 465, 170);
	Rect gameLettersRect = new Rect (225, 180, 300, 170);
	Rect gameTimeRect = new Rect (250, 280, 250, 170);

	Rect optionsFontSizeRect = new Rect (465, 30, 150, 35);
	Rect optionsChallengeRect = new Rect (150, 80, 230, 430);
	Rect optionsSurvivalRect = new Rect (415, 80, 200, 170);
	Rect optionsResetRect = new Rect (300, 400, 250, 170);

	Rect aboutRect = new Rect (150, 80, 465, 300);

	public GUISkin greenStyle;
	public GUISkin blueStyle;
	public GUISkin redStyle;
	public GUISkin grayStyle;

	public bool isStyled;

	void OnGUI(){
		GUI.skin = myGUISkin;
		// To turn off all styling
		if(isStyled == false){
			greenStyle = myGUISkin;
			blueStyle = myGUISkin;
			redStyle = myGUISkin;
			grayStyle = myGUISkin;
		}

		optionsSurvivalRect = new Rect (400, 180, 215, 370);

		// Scaling the GUI
		myGUISkin.box.fontSize = scrolled * 10;
		myGUISkin.label.fontSize = scrolled * 10;
		myGUISkin.button.fontSize = scrolled * 10;
		myGUISkin.textField.fontSize = scrolled * 10;

		blueStyle.box.fontSize = scrolled * 10;
		greenStyle.box.fontSize = scrolled * 10;
		redStyle.button.fontSize = scrolled * 10;
		blueStyle.button.fontSize = scrolled * 10;
		greenStyle.button.fontSize = scrolled * 10;
		grayStyle.textField.fontSize = scrolled * 10;
		grayStyle.box.fontSize = scrolled * 10;
		

		// Back button goes back to menu or quits if on menu
		if(GUI.Button(backButtonRect, "<- Back", redStyle.button)){
			if(gameState == (int)GameStateEnum.MENU){
				Application.Quit();
			}
			else {
				gameState = (int)GameStateEnum.MENU;
			}
		}

		// Buttons used to scale the GUI
		GUILayout.BeginArea(scalingButtonsRect); // Keep the buttons in the right place
		GUILayout.BeginHorizontal(); // Make the sit side by side
		if(GUILayout.Button("A-", blueStyle.button)){
			if(scrolled > scrollMin){
				scrolled--;
			}
		}
		if(GUILayout.Button("A+", blueStyle.button)){
			if(scrolled < scrollMax){
				scrolled++;
			}
		}
		//myGUISkin.label.fontSize = scrolled * 10; // * 10 otherwise don't see text
		GUILayout.EndHorizontal();
		GUILayout.EndArea();



		/// Main menu
		/// Select challenge on left.
		/// Play game and go to other menu's on right.
		/// Back button quits.
		/// A- and A+ maybe useless or moved to options.
		if(gameState == (int)GameStateEnum.MENU){

			// Challenges pages options buttons
			GUILayout.BeginArea(menuChallengeRect);
			GUILayout.BeginHorizontal(); // Keep page buttons together
			if(GUILayout.Button("Prev", blueStyle.button)){
				if(challengePage > 1){
					challengePage--;
				}
			}
			GUILayout.TextField(challengePage.ToString(), grayStyle.textField);
			if(GUILayout.Button("Next", blueStyle.button)){
				if(challengePage < challengePageTotal){
					challengePage++;
				}
			}
			GUILayout.EndHorizontal();

			// Challenges
			string[] stringArray = {challenge1Name, challenge2Name, challenge3Name, challenge4Name,
			challenge5Name, challenge6Name, challenge7Name, challenge8Name};

			/* alternative for changing button when clicked
			if(challengeSelected == 1)
			 */

			// Challenges page 1
			if(challengePage == 1){
				// 4 challenges on this page
				for(int i = 0; i < 4; i++){
					// One button for each challenge
					// Name on button is challenge name
					if(GUILayout.Button(stringArray[i], greenStyle.button, GUILayout.Height(65))){
						// Challenge number is one above array position
						int j = i + 1;
						setChallenge(j.ToString());
					}
				}
			}

			// Challenges page 2
			if(challengePage == 2){
				for(int i = 4; i < 8; i++){
					if(GUILayout.Button(stringArray[i], greenStyle.button, GUILayout.Height(65))){
						int j = i + 1;
						setChallenge(j.ToString());
					}
				}
			}

			// Challenge description and letters
			// Temp to fix double the number challenge
			if(challengeSelected == 8){
				//string myString = string.Join("", challenge8Letters);
				GUILayout.TextField("Description\n" + 
				                    challengeDescription +
				                    "\n\nLetters\n" + 
				                    string.Join(", ", challenge8Letters), grayStyle.textField);
			}
			else{
			GUILayout.TextField("Description\n" + 
			                    challengeDescription +
			                	"\n\nLetters\n" + 
				                challengeLetters, grayStyle.textField);
			}
			GUILayout.EndArea();


			// Menu buttons
			GUILayout.BeginArea(menuButtonsRect);
			if(GUILayout.Button("Play", blueStyle.button, GUILayout.Height(65))){
				gameState = (int)GameStateEnum.GAME;
			}
			if(GUILayout.Button("Options", blueStyle.button, GUILayout.Height(65))){
				gameState = (int)GameStateEnum.OPTIONS;
			}
			if(GUILayout.Button("About", blueStyle.button, GUILayout.Height(65))){
				gameState = (int)GameStateEnum.ABOUT;
			}
			if(GUILayout.Button("Survival", blueStyle.button, GUILayout.Height(65))){
				gameState = (int)GameStateEnum.GAME;
				isSurvivalMode = true;
				setChallenge("survival");
			}
			if(GUILayout.Button("Custom Game", blueStyle.button, GUILayout.Height(65))){
				gameState = (int)GameStateEnum.GAME;
				setChallenge("custom");
			}
			GUILayout.EndArea();


			// Move custom name field to bottom right of menu buttons
			Rect customNameFieldRect = menuButtonsRect;
			customNameFieldRect.y += 295;
			customNameFieldRect.x += 205;
			// Area for custom name field
			GUILayout.BeginArea(customNameFieldRect);
			customName = GUILayout.TextField(customName, grayStyle.textField, GUILayout.Width(160));
			GUILayout.EndArea();
		}
		
		







		/// Game menu
		/// bla bla bla bla
		/// bla bla bla bla
		/// bla bla bla bla
		/// bla bla bla bla
		if(gameState == (int)GameStateEnum.GAME){
			// Game challenge and description
			GUILayout.BeginArea(gameChallengeRect);
			GUILayout.Box("Challenge - " + challengeName, blueStyle.box);
			GUILayout.Box(challengeDescription + "", greenStyle.box);
			GUILayout.EndArea();

			// Game letter
			GUILayout.BeginArea(gameLettersRect);
			// Display current letter and time
			GUILayout.BeginHorizontal();
			if(!isFinished){
				GUILayout.Box("Type:", blueStyle.box, GUILayout.Height(70));
				// When on a challenge that shows word not letter, display the word.
				if(challengeSelected == 6 || challengeSelected == 7 || challengeSelected == 8){
					string myString = challengeLetters.Substring(letterNum);
					GUILayout.Box(myString.ToUpper(), greenStyle.box);
				}
				else{
					GUILayout.Box(letter.ToString().ToUpper(), greenStyle.box, GUILayout.Height(70));
				}
			}
			else{
				GUILayout.Box("Finished", blueStyle.box, GUILayout.Height(70));
			}
			// Survival mode?
			if(isSurvivalMode && !isFinished){
				GUILayout.Box(survivalTime.ToString("00.00"), greenStyle.box);
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();


			// Game time
			GUILayout.BeginArea(gameTimeRect);
			GUILayout.Box("Press Space bar to restart", greenStyle.box);
			GUILayout.BeginHorizontal();
			GUILayout.Box("Time\n" + time.ToString("00.00"), blueStyle.box);
			GUILayout.Space(20);
			// dispay other scores
			float myBestTime = PlayerPrefs.GetFloat(scoresKey);
			GUILayout.Box("Best\n" + myBestTime.ToString("00.00"), blueStyle.box);
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			
			
			// Submit score
			if(isFinished){
				if(!isSurvivalMode){
					if(PlayerPrefs.GetFloat(scoresKey) > time || !PlayerPrefs.HasKey(scoresKey)){
						PlayerPrefs.SetFloat(scoresKey, time);
						PlayerPrefs.Save();
					}
				}
				else{
					// Survival time save
					if(PlayerPrefs.GetFloat(scoresKey) < time || !PlayerPrefs.HasKey(scoresKey)){
						PlayerPrefs.SetFloat(scoresKey, time);
						PlayerPrefs.Save();
					}
				}
			}
		}
		






		/// Options menu
		/// bla bla bla bla
		/// bla bla bla bla
		/// bla bla bla bla
		/// bla bla bla bla
		if(gameState == (int)GameStateEnum.OPTIONS){			
			// Font size
			GUILayout.BeginArea(optionsFontSizeRect);
			GUILayout.Box("Font size: " + scrolled, grayStyle.box);
			GUILayout.EndArea();

			// Here for now
			float myBestTime = PlayerPrefs.GetFloat(scoresKey);

			// Challenges
			GUILayout.BeginArea(optionsChallengeRect);
			// Challenges
			string[] stringArray = {challenge1Name, challenge2Name, challenge3Name, challenge4Name,
				challenge5Name, challenge6Name, challenge7Name, challenge8Name};
			
			/* alternative for changing button when clicked
			if(challengeSelected == 1)
			 */
			
			// Challenges page 1 
			if(challengePage == 1){
				// 4 challenges on this page
				for(int i = 0; i < 4; i++){
					int j = i + 1;
					// One button for each challenge
					// Name on button is challenge name
					// Challenge button on left, reset and time on right.
					GUILayout.BeginHorizontal(); 
					if(GUILayout.Button(stringArray[i], greenStyle.button, 
					                    				GUILayout.Height(65), 
					                    				GUILayout.Width(145))){
						// Challenge number is one above array position
						setChallenge(j.ToString());
					}

					// Reset score button and best time
					GUILayout.BeginVertical(); // Reset button and best time together
					// Reset score button
					if(GUILayout.Button("Reset score", blueStyle.button)){
						//PlayerPrefs.DeleteKey(scoresKey);
					}
					// Best time
					scoresKey = "challenge" + int.Parse(j.ToString());
					myBestTime = PlayerPrefs.GetFloat(scoresKey);
					GUILayout.Box("Best time:\n" + myBestTime.ToString("00.00"), grayStyle.box);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
			}
			
			// Challenges page 2
			if(challengePage == 2){
				for(int i = 4; i < 8; i++){
					int j = i + 1;
					GUILayout.BeginHorizontal();
					if(GUILayout.Button(stringArray[i], greenStyle.button, 
					                    				GUILayout.Height(65), 
					                   					GUILayout.Width(145))){
						setChallenge(j.ToString());
					}

					// Reset score button and best time
					GUILayout.BeginVertical(); // Reset button and best time together
					// Reset score button
					if(GUILayout.Button("Reset score", blueStyle.button)){
						//PlayerPrefs.DeleteKey(scoresKey);
					}
					// Best time
					scoresKey = "challenge" + int.Parse(j.ToString());
					myBestTime = PlayerPrefs.GetFloat(scoresKey);
					GUILayout.Box("Best time:\n" + myBestTime.ToString("00.00"), grayStyle.box);
					GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				}
			}


			// Challenges pages options buttons
			GUILayout.BeginHorizontal(); // Keep page buttons together
			if(GUILayout.Button("Prev", blueStyle.button)){
				if(challengePage > 1){
					challengePage--;
				}
			}
			GUILayout.TextField(challengePage.ToString(), grayStyle.textField);
			if(GUILayout.Button("Next", blueStyle.button)){
				if(challengePage < challengePageTotal){
					challengePage++;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			///Progress////////////////////////////////
			/// Last progress was fixing challenges /// 
			/// pages on options page				///
			///////////////////////////////////////////
			// Survival
			GUILayout.BeginArea(optionsSurvivalRect);
			GUILayout.BeginHorizontal(); 
			if(GUILayout.Button("Survival", greenStyle.button, 
			                    			GUILayout.Height(65), 
			                  				GUILayout.Width(130))){
				setChallenge("survival");
			}
			
			// Reset score button and best time
			GUILayout.BeginVertical(); // Reset button and best time together
			// Reset score button
			if(GUILayout.Button("Reset score", blueStyle.button)){
				//PlayerPrefs.DeleteKey(scoresKey);
			}
			// Best time
			scoresKey = "survival";
			myBestTime = PlayerPrefs.GetFloat(scoresKey);
			GUILayout.Box("Best time:\n" + myBestTime.ToString("00.00"), grayStyle.box);
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();

			// Survival mode options, do not show unless I have set it to
			if(isSurvivalMode && showSurvivalOptions == true){
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("-")){survivalTimeStart -= 0.2f;}
				GUILayout.Box("Starting time: " + survivalTimeStart.ToString());
				if(GUILayout.Button("+")){survivalTimeStart += 0.2f;}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("-")){survivalAddStart -= 0.2f;}
				GUILayout.Box("Add to time: " + survivalAddStart.ToString());
				if(GUILayout.Button("+")){survivalAddStart += 0.2f;}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("-")){survivalAddLowLimit -= 0.2f;}
				GUILayout.Box("Lowest add to time: " + survivalAddLowLimit.ToString());
				if(GUILayout.Button("+")){survivalAddLowLimit += 0.2f;}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("-")){survivalAddToMinus -= 0.2f;}
				GUILayout.Box("Degrading add: " + survivalAddToMinus.ToString());
				if(GUILayout.Button("+")){survivalAddToMinus += 0.2f;}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			// Reset all scores
			GUILayout.BeginArea(optionsResetRect);
			if(GUILayout.Button("Reset all scores", blueStyle.button)){
				resetAllScores = !resetAllScores;
			}
			// When reset is pressed
			if(resetAllScores == true){
				if(GUILayout.Button("Are you sure?", blueStyle.button)){
					//PlayerPrefs.DeleteAll();
					resetAllScores = !resetAllScores;
				}
			}
			GUILayout.EndArea();
		}
		
		
		
		/// About menu
		/// bla bla bla bla
		/// bla bla bla bla
		/// bla bla bla bla
		/// bla bla bla bla
		if(gameState == (int)GameStateEnum.ABOUT){
			GUILayout.BeginArea(aboutRect);
			GUILayout.Box(aboutInfo, grayStyle.box);
			GUILayout.EndArea();
		}
	}

	void reset(){
		time = 0;
		if(isSurvivalMode == false){
			if(challengeSelected == 6){
				challengeLetters = challenge6Letters[Random.Range(0, challenge6Letters.Length-1)];
				challengeDescription = "Type \"" + challengeLetters.ToUpper() + "\"";
			}
			if(challengeSelected == 7){
				challengeLetters = challenge7Letters[Random.Range(0, challenge7Letters.Length-1)];
				challengeDescription = "Type \"" + challengeLetters.ToUpper() + "\"";
			}
			if(challengeSelected == 8){
				challengeLetters = challenge8Letters[0];
				//challengeLetters = challenge8Letters[Random.Range(0, challenge8Letters.Length-1)];
				challenge8Pos = 0;
			}
			letterNum = 0;
			letter = challengeLetters[letterNum];
			
		}
		// Survival restart
		else{
			letter = challenge1Letters[Random.Range(0, challenge1Letters.Length-1)];
			survivalTime = survivalTimeStart;
			survivalAdd = survivalAddStart;
		}
		isRunning = false;
		isFinished = false;
	}

	void setChallenge(string challenge){
		// Setup one for all

		/*
		// Get the number from challenge string
		string temp = "";
		for (int i=0; i<challenge.Length; i++)
		{
			if (char.IsDigit(challenge[i])){
				temp += challenge[i];
			}
		}
		if (temp.Length > 0){
			challengeNum = int.Parse(temp);
		}
		else{
			
		}
		challengeNum = int.Parse(temp);
		*/

		// Use TryParse as custom and survival do not have numbers
		//challengeSelected = int.Parse(challenge);
		int.TryParse(challenge, out challengeSelected);
		scoresKey = "challenge" + challengeSelected;

		// Setting survival
		if(challenge == "survival"){
			challengeName = "survival";
			challengeLetters = "survival";
			challengeDescription = "Type until the time runs out";
			scoresKey = "survival";
			
			isSurvivalMode = true;
		}
		else{
			isSurvivalMode = false;
		}

		// Setting custom
		if(challenge == "custom"){
			challengeName = "custom";
			challengeLetters = customName.ToLower();
			challengeDescription = "Type \"" + customName.ToUpper() + "\"";
			scoresKey = "custom";
		}

		// Setting challenge 1-8
		switch(challengeSelected){
			case 1:
				challengeName = challenge1Name; 
				challengeLetters = challenge1Letters;
				challengeDescription = challenge1Description;
				break;
			case 2:
				challengeName = challenge2Name;
				challengeLetters = challenge2Letters;
				challengeDescription = challenge2Description;
				break;
			case 3:
				challengeName = challenge3Name;
				challengeLetters = challenge3Letters;
				challengeDescription = challenge3Description;
				break;
			case 4:
				challengeName = challenge4Name;
				challengeLetters = challenge4Letters;
				challengeDescription = challenge4Description;
				break;
			case 5:
				challengeName = challenge5Name;
				challengeLetters = challenge5Letters;
				challengeDescription = challenge5Description;
				break;
			case 6:
				challengeName = challenge6Name;
				challengeLetters =  challenge6Letters[Random.Range(0, challenge6Letters.Length-1)];
				challengeDescription = "Type \"" + challengeLetters.ToUpper() + "\"";
				// Challenge description?
				break;
			case 7:
				challengeName = challenge7Name;
				challengeLetters = challenge7Letters[Random.Range(0, challenge7Letters.Length-1)];
				challengeDescription = challenge7Description;
				break;
			case 8:
				challengeName = challenge8Name;
				//challengeLetters = string.Join("", challenge8Letters);
				//challengeLetters = challenge8Letters[Random.Range(0, challenge8Letters.Length-1)];
				challengeLetters = challenge8Letters[0];
				challengeDescription = challenge8Description;
				break;
		}
		reset();
	}
}
