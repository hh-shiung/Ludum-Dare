﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState
{
	Playing,
	End,
}

public enum PowerUp
{
	IncreaseLength,
	DecreaseLength,
	IncreaseSpeed,
	DecreaseSpeed,
}

public class GameManager : MonoBehaviour {

	public GameObject playerPrefab;

	public GameObject HUDCanvas; 

	public GameObject playerHUD;

	public GameObject[] tiles; 

	public GameObject wall;

	public GameObject deco;

	public GameState currentState;

	public float gameOverTime = 3.0f;

	float powerUPTime = 5.0f;

	float powerUpTimer = 0.0f;

	float borderWidth = 20;

	float borderHeight = 20;

	List<Player> players;

	List<GameObject> walls;

	// Use this for initialization
	void Start () {
		currentState = GameState.Playing;

		players = new List<Player>();

		walls = new List<GameObject>();

		GenerateBackgroundTiles(borderWidth, borderHeight);


		AddPlayer(KeyCode.LeftArrow, KeyCode.RightArrow);

		AddPlayer(KeyCode.A, KeyCode.D);

		AddPlayer(KeyCode.Q, KeyCode.E);

		AddPlayer(KeyCode.Z, KeyCode.C);
	}
	
	// Update is called once per frame
	void Update () {
		if(currentState == GameState.Playing)
		{
			if(powerUpTimer > powerUPTime)
			{
				GeneratePowerUps();
				powerUpTimer = 0.0f;
			}
			else
			{
				powerUpTimer += Time.deltaTime;
			}

			foreach(Player player in players)
			{				
				if(player.gameOverTimer > gameOverTime)
				{
					currentState = GameState.End;
					player.AnimateDead();
					Debug.Log("Player " + player.playerIndex + " lose!!");
				}
			}
		}
	}

	public bool CheckNextPosition(int playerIndex, GameObject head, Direction playerDirection)
	{
		Vector3 nextPos = head.transform.position;
		if(playerDirection == Direction.Up)
		{
			nextPos += new Vector3(0, 1, 0);
		}
		else if(playerDirection == Direction.Down)
		{
			nextPos -= new Vector3(0, 1, 0);
		}
		else if(playerDirection == Direction.Left)
		{
			nextPos -= new Vector3(1, 0, 0);
		}
		else
		{
			nextPos += new Vector3(1, 0, 0);
		}

		foreach(Player player in players)
		{
			if(player.CheckPositionBelongsToPlayer(nextPos))
			{
				//TODO show timer
				return false;
			}
		}

		foreach(GameObject wall in walls)
		{
			if((Vector2)(wall.transform.position) == (Vector2)(nextPos))
			{
				return false;
			}
		}

		if(CheckIfPositionExceedBorder(nextPos))
			return false;

		return true;
	}

	bool CheckIfPositionExceedBorder(Vector3 position)
	{
		
		if(position.x < borderWidth / 2 * -1 ||
			position.x >= borderWidth / 2 ||
			position.y < borderHeight / 2 * -1 ||
			position.y >= borderHeight / 2)		
		{
//			Debug.Log("Position is " + position + " " + borderWidth / 2 * -1);
			return true;
		}

		return false;
	}

	void GenerateBackgroundTiles(float width, float height)
	{
		int maxRockCount = Random.Range(4, 10);
		for(int w = (int)(width / 2 * -1) ; w < (int)(width / 2) ; w++)
		{
			for(int h = (int)(height / 2 * -1) ; h < (int)(height / 2) ; h++)
			{
				GameObject tile = Instantiate(tiles[Random.Range(0, tiles.Length)]);
				tile.transform.position = new Vector3(w, h, 1);

				//add deco
				int r = Random.Range(0, 50);
				if(r == 15)
				{
					GameObject tileDeco = Instantiate(deco);
					tileDeco.transform.position = new Vector3(w, h, 0.5f);
				}
				else if(r == 40)
				{
					if(walls.Count < maxRockCount)
					{
						GameObject rock = Instantiate(wall);
						rock.transform.position = new Vector3(w, h, 0);

						walls.Add(rock);
					}						
				}
			}
		}
	}

	void AddPlayer(KeyCode leftKey, KeyCode rightKey)
	{
		Vector3 playerInitialPos;

		if(players.Count == 0)
		{
			playerInitialPos = new Vector3(5, 0, 0);
		}
		else if(players.Count == 1)
		{
			playerInitialPos = new Vector3(0, 5, 0);
		}
		else if(players.Count == 2)
		{
			playerInitialPos = new Vector3(-5, 0, 0);
		}
		else
		{
			playerInitialPos = new Vector3(0, -5, 0);
		}

		GameObject player = Instantiate(playerPrefab);
		player.transform.position = playerInitialPos;

		Player p = player.GetComponent<Player>();
		p.gameManager = this;
		p.playerIndex = players.Count;
		p.left = leftKey;
		p.right = rightKey;

		players.Add(p);

		int width = 150;
		int initialX = 80;
		int initialY = 525;
		GameObject HUD = Instantiate(playerHUD);
		HUD.transform.SetParent(HUDCanvas.transform);
		HUD.transform.position = new Vector3(initialX + p.playerIndex * width, initialY, 0);

		PlayerHUD pHUD = HUD.GetComponent<PlayerHUD>();
		pHUD.SetupHUD("Player " + p.playerIndex, p.playerIndex);

		p.HUD = pHUD;
	}

	void GeneratePowerUps()
	{

	}
}
