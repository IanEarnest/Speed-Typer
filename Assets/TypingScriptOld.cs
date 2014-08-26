using UnityEngine;
using System.Collections;

public class TypingScriptOld : MonoBehaviour {

	/* Improvements
	//i changed challenge 6 and 7 so you can see words instead of letters and word changes on reset.
	Limit options and buttons shown.
	Show a picture of a keyboard and the key to press.
	Penalty for typing the wrong letter on all modes.
	Show a timer of two seconds or something that resets on each key press.
	Create typing words which show what part you have typed.
	*/

	/* Create a selection of challenges.
	Challenge 1: Alphabet
	Challenge 2: Alphabet backwards
	Challenge 3: Quick brown fox
	Challenge 4: Numbers 1 - 9
	Challenge 5: Numbers 1 - 20
	Challenge 6: Different names
	Challenge 7: Made up names
	*/

	string challenge1 = "abcdefghijklmnopqrstuvwxyz";
	string challenge2 = "zyxwvutsrqponmlkjihgfedcba";
	string challenge3 = "thequickbrownfoxjumpsoverthelazydog";
	string challenge4 = "0123456789";
	string challenge5 = "01234567891011121314151617181920";
	string[] challenge6 = {"steve", "alan", "john", "edward", "kevin", "ken"};
	string[] challenge7 = {"asdanoin", "ownikibqw", "oluijni", "aaikiisd", "iwnuiwrg", "jhni"};
	int challenges = 7;
	string challengeText;
	string challengeHint;
	int challengeNum;
	string scoresKey;
	string customName = "customName";

	int letterNum = 0;
	char letter;
	float time;
	float survivalTime;
	float survivalTimeStart = 1;
	float survivalAdd;
	float survivalAddStart = 1;
	float survivalAddLowLimit = 0.2f;
	float survivalAddToMinus = 0.1f;

	bool isRunning;
	bool isFinished = true;
	//bool showTimer;
	bool showScores = true;
	bool showOptions = true;
	bool isSurvival;


	public GUISkin myGUISkin;

	#if UNITY_ANDROID
	TouchScreenKeyboard keyboard;
	#endif

	int scrolled;
	int scrollMin = 1;
	int scrollMax = 15;

	// Use this for initialization
	void Start () {
		setChallenge("1");

		// check screen size, change scrolled. 1050, 1680 = 3. 1920, 1080 = 6
		scrolled = Screen.height / 200;

		#if UNITY_ANDROID
		TouchScreenKeyboard.hideInput = true;
		keyboard = TouchScreenKeyboard.Open("");
		Screen.orientation = ScreenOrientation.AutoRotation;
		#endif

		// Show options at start if player has never played.
		for(int i=1; i<challenges; i++){
			if(PlayerPrefs.HasKey("challenge" + i)){
				// player has played before.
				showOptions = false;
			}
		}

		reset();
	}

	// Update is called once per frame
	void Update () {
		// add to time
		if(isRunning){
			time += Time.deltaTime;
			if(isSurvival && survivalTime > 0){
				survivalTime -= Time.deltaTime;
			}
			else if (isSurvival && survivalTime <= 0){
				isRunning = false;
				isFinished = true;
			}
		}
		// Press currect letter
		if(!isFinished){
			#if UNITY_ANDROID
			if(Input.GetKeyDown(letter.ToString()) || 
			   keyboard.text.ToLower() == letter.ToString()){
			#else
			if(Input.GetKeyDown(letter.ToString())){ 
			#endif
				isRunning = true;
	
				if(!isSurvival){
					letterNum++;
		
					// Not finished challenge
					if(letterNum != challengeText.Length){
							// Timer continues if user presses last key a second time
					// Fixed with this if else
						if(letterNum < challengeText.Length){
							letter = challengeText[letterNum];
						}
						else{
							isRunning = false;
							isFinished = true;
						}
					}
					// end of challenge
					else{
						isRunning = false;
						isFinished = true;
					}
				}
				// Survival
				else{
					// Random letter
					letter = challenge1[Random.Range(0, challenge1.Length-1)];
					survivalTime += survivalAdd;
					if(survivalAdd > survivalAddLowLimit){
						survivalAdd -= survivalAddToMinus;
					}
				}
			}
		}

		#if UNITY_ANDROID
		// Click to show keyboard
		if(!keyboard.active && Input.GetMouseButtonDown(0)){
			keyboard = TouchScreenKeyboard.Open("");
		}

		// Space to reset game
		if(Input.GetKeyDown(KeyCode.Space) || 
		   keyboard.text == " "){
			reset();
		}
		#else
		// Space to reset game
		if(Input.GetKeyDown(KeyCode.Space)){
			reset();
		}
		#endif

		// Escape/Back button to quit
		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
			#if UNITY_ANDROID
			keyboard.active = false;
			#endif
		}

		// Dont need this as it is in GUI
		#if UNITY_ANDROID
		//keyboard.text = "";
		#endif
	}

	void mainWindow(int id){
		myGUISkin.box.fontSize = scrolled * 10;
		myGUISkin.label.fontSize = scrolled * 10;
		myGUISkin.button.fontSize = scrolled * 10;
		myGUISkin.textField.fontSize = scrolled * 10;

		if(showOptions){
			// Keep scrolled value always same size
			myGUISkin.label.fontSize = 15;
			// Scroller to adjust GUI size
			scrolled = (int)GUILayout.HorizontalSlider(scrolled, scrollMin, scrollMax);
			GUI.Label(new Rect(Screen.width/2, 0, 150, 50), scrolled.ToString());
			myGUISkin.label.fontSize = scrolled * 10;
			GUILayout.Label("Slider above changes text size.");

			// Display keyboard information
			#if UNITY_ANDROID
			if(!keyboard.active){
				GUILayout.Label("Click the screen to display keyboard.");
			}
			else{
				GUILayout.Label("Press enter on keyboard to close it.");
			}
			#endif

			if(GUILayout.Button("Play")){
				showOptions = !showOptions;
			}
			
			/*
			GUILayout.BeginHorizontal();
			// See current time score in play
			if(GUILayout.Button("Time")){
				showTimer = !showTimer;
			}
			// See current high score
			if(GUILayout.Button("Scores")){
				showScores = !showScores;
			}
			GUILayout.EndHorizontal();
			*/

			if(GUILayout.Button("Reset score")){
				PlayerPrefs.DeleteKey(scoresKey);
			}
			if(GUILayout.Button("Reset all scores")){
				PlayerPrefs.DeleteAll();
			}
			
			GUILayout.Box("Challenges");
			GUILayout.BeginHorizontal();
			if(challengeNum == 1){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("1")){ setChallenge("1");}
			if(challengeNum == 1){ GUILayout.Label("-", GUILayout.Width(7));}

			if(challengeNum == 2){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("2")){ setChallenge("2");}
			if(challengeNum == 2){ GUILayout.Label("-", GUILayout.Width(7));}
			
			if(challengeNum == 3){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("3")){ setChallenge("3");}
			if(challengeNum == 3){ GUILayout.Label("-", GUILayout.Width(7));}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				if(challengeNum == 4){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("4")){ setChallenge("4");}
				if(challengeNum == 4){ GUILayout.Label("-", GUILayout.Width(7));}

				if(challengeNum == 5){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("5")){ setChallenge("5");}
				if(challengeNum == 5){ GUILayout.Label("-", GUILayout.Width(7));}

				if(challengeNum == 6){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("6")){ setChallenge("6");}
				if(challengeNum == 6){ GUILayout.Label("-", GUILayout.Width(7));}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				if(challengeNum == 7){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("7")){ setChallenge("7");}
				if(challengeNum == 7){ GUILayout.Label("-", GUILayout.Width(7));}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
				if(challengeNum == 8){ GUILayout.Label("-", GUILayout.Width(7));}
				if(GUILayout.Button("Survival")){ setChallenge("8");}
				if(challengeNum == 8){ GUILayout.Label("-", GUILayout.Width(7));}
			GUILayout.EndHorizontal();

				if(isSurvival){
					GUILayout.BeginHorizontal();
					if(GUILayout.Button("-")){survivalTimeStart -= 0.2f;}
					GUILayout.Label("Starting time: " + survivalTimeStart.ToString());
					if(GUILayout.Button("+")){survivalTimeStart += 0.2f;}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					if(GUILayout.Button("-")){survivalAddStart -= 0.2f;}
					GUILayout.Label("Add to time: " + survivalAddStart.ToString());
					if(GUILayout.Button("+")){survivalAddStart += 0.2f;}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					if(GUILayout.Button("-")){survivalAddLowLimit -= 0.2f;}
					GUILayout.Label("Lowest add to time: " + survivalAddLowLimit.ToString());
					if(GUILayout.Button("+")){survivalAddLowLimit += 0.2f;}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					if(GUILayout.Button("-")){survivalAddToMinus -= 0.2f;}
					GUILayout.Label("Degrading add: " + survivalAddToMinus.ToString());
					if(GUILayout.Button("+")){survivalAddToMinus += 0.2f;}
					GUILayout.EndHorizontal();
				}
				/*
			GUILayout.BeginHorizontal();
			GUI.SetNextControlName("Input");
			customName = GUILayout.TextField(customName, 30);
			
			#if UNITY_ANDROID
				if(GUI.GetNameOfFocusedControl() != "Input"){
					keyboard.text = "";
				}
				else{
					customName = keyboard.text;
				}
			#endif
			
			//not working, text being cleared
			if(challengeNum == 9){ GUILayout.Label("-", GUILayout.Width(7));}
			if(GUILayout.Button("Custom game")){
				if(customName != ""){
					setChallenge("9");
				}
			}
			if(challengeNum == 9){ GUILayout.Label("-", GUILayout.Width(7));}
			GUILayout.EndHorizontal();
			*/
			GUILayout.Label(challengeHint);
			GUILayout.Label(challengeText);
			float myBestTime = PlayerPrefs.GetFloat(scoresKey);
			GUILayout.Label("Best: " + myBestTime.ToString("00.00"));
		}

		
		if(!showOptions){
			bool tempKeyboard = true;
			#if UNITY_ANDROID
			if(!keyboard.active){
				GUILayout.Label("Click the screen to display keyboard.");
				tempKeyboard = false;
			}
			#endif
			// to allow testing
			#if UNITY_EDITOR
			tempKeyboard = true;
			#endif
			// When keyboard is not displayed, do not show game

			if(tempKeyboard){
				GUILayout.Label(challengeHint);
				
				// Display current letter and time
				GUILayout.BeginHorizontal();
				if(!isFinished){
					// When on a challenge that shows word not letter, display the word.
					if(challengeNum == 6 || challengeNum == 7){
						string myString = challengeText.Substring(letterNum);
							GUILayout.Box(myString.ToUpper());
					}
					else{
						GUILayout.Box(letter.ToString().ToUpper());
					}
				}
				if(isSurvival && !isFinished){
					GUILayout.Box(survivalTime.ToString("00.00"));
				}
				// Finished
				if(isFinished /*|| showTimer*/){
					GUILayout.Box(time.ToString("00.00"));
				}		
				
				GUILayout.EndHorizontal();

				
				// dispay other scores
				if(isFinished || showScores){
					float myBestTime = PlayerPrefs.GetFloat(scoresKey);
					GUILayout.Box("Best: " + myBestTime.ToString("00.00"));
				}

				GUILayout.Label("Press Space bar to restart");

				if(GUILayout.Button("Options")){
					showOptions = !showOptions;
				}

				// Submit score
				if(isFinished){
					if(!isSurvival){
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
		}
	}
	
	void OnGUI(){
		GUI.skin = myGUISkin;
		Rect mainWindowRect = new Rect(0, 0, Screen.width, Screen.height);
		GUILayout.Window (0, mainWindowRect, mainWindow, "");
	}

	void reset(){
		time = 0;
		if(!isSurvival){
			if(challengeNum == 6){
				challengeText =  challenge6[Random.Range(0, challenge6.Length-1)];
				challengeHint = "Type \"" + challengeText.ToUpper() + "\"";
			}
			if(challengeNum == 7){
				challengeText =  challenge7[Random.Range(0, challenge7.Length-1)];
				challengeHint = "Type \"" + challengeText.ToUpper() + "\"";
			}
			letterNum = 0;
			letter = challengeText[letterNum];
			
		}
		else{
			letter = challenge1[Random.Range(0, challenge1.Length-1)];
			survivalTime = survivalTimeStart;
			survivalAdd = survivalAddStart;
		}
		isRunning = false;
		isFinished = false;
	}

	void setChallenge(string challenge){
		if(int.Parse(challenge) == 1){
			challengeNum = int.Parse(challenge);
			challengeText = challenge1;
			challengeHint = "Type the alphabet";
			scoresKey = "challenge" + int.Parse(challenge);
		}
		if(int.Parse(challenge) == 2){
			challengeNum = int.Parse(challenge);
			challengeText = challenge2;
			challengeHint = "Type the alphabet backwards";
			scoresKey = "challenge" + int.Parse(challenge);
		}
		if(int.Parse(challenge) == 3){
			challengeNum = int.Parse(challenge);
			challengeText = challenge3;
			challengeHint = "Classic typing exercise";
			scoresKey = "challenge" + int.Parse(challenge);
		}
		if(int.Parse(challenge) == 4){
			challengeNum = int.Parse(challenge);
			challengeText = challenge4;
			challengeHint = "Type 0-9";
			scoresKey = "challenge" + int.Parse(challenge);
		}
		if(int.Parse(challenge) == 5){
			challengeNum = int.Parse(challenge);
			challengeText = challenge5;
			challengeHint = "Type 0-20";
				scoresKey = "challenge" + int.Parse(challenge);
		}
		if(int.Parse(challenge) == 6){
			challengeNum = int.Parse(challenge);
			challengeText =  challenge6[Random.Range(0, challenge6.Length-1)];
			challengeHint = "Type \"" + challengeText.ToUpper() + "\"";
			scoresKey = "challenge" + int.Parse(challenge);
		}
		if(int.Parse(challenge) == 7){
			challengeNum = int.Parse(challenge);
			challengeText = challenge7[Random.Range(0, challenge7.Length-1)];;
			challengeHint = "Type \"" + challengeText.ToUpper() + "\"";
			scoresKey = "challenge" + int.Parse(challenge);
		}

		if(int.Parse(challenge) == 8){
			challengeNum = int.Parse(challenge);
			challengeText = "survival";
			challengeHint = "Type untill time runs out";
			scoresKey = "survival";
			
			isSurvival = true;
		}
		else{
			isSurvival = false;
		}
		/*
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
		
		if(int.Parse(challenge) == 9){
			challengeNum = int.Parse(challenge);
			challengeText = customName.ToLower();
			challengeHint = "Type \"" + customName.ToUpper() + "\"";
			scoresKey = "custom";
		}
		reset();
	}
}
