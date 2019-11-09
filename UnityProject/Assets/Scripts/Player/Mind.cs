﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// IC character information (job role, antag info, real name, etc). A body and their ghost link to the same mind
/// </summary>
public class Mind
{
	public Occupation occupation;
	public PlayerScript ghost;
	public PlayerScript body;
	public bool IsGhosting;
	public bool DenyCloning;
	public int bodyMobID;

	//use Create to create a mind.
	private Mind()
	{
	}

	/// <summary>
	/// Creates and populates the mind for the specified player.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="occupation"></param>
	public static void Create(GameObject player, Occupation occupation)
	{
		var mind = new Mind {occupation = occupation};
		var playerScript = player.GetComponent<PlayerScript>();
		mind.SetNewBody(playerScript);
	}

	public void SetNewBody(PlayerScript playerScript){
		playerScript.mind = this;
		body = playerScript;
		bodyMobID = playerScript.GetComponent<LivingHealthBehaviour>().mobID;
		StopGhosting();
	}
	public GameObject GetCurrentMob()
	{
		if (IsGhosting)
		{
			return ghost.gameObject;
		}
		else
		{
			return body.gameObject;
		}
	}

	public void Ghosting(GameObject newGhost)
	{
		IsGhosting = true;
		var PS = newGhost.GetComponent<PlayerScript>();
		PS.mind = this;
		ghost = PS;
	}

	public void StopGhosting()
	{
		IsGhosting = false;
	}

	public bool ConfirmClone(int recordMobID)
	{
		if(bodyMobID != recordMobID){  //an old record might still exist even after several body swaps
			return false;
		}
		if(DenyCloning)
		{
			return false;
		}
		var currentMob = GetCurrentMob();
		if(!IsGhosting)
		{
			var livingHealthBehaviour = currentMob.GetComponent<LivingHealthBehaviour>();
			if(!livingHealthBehaviour.IsDead)
			{
				return false;
			}
		}
		if(!IsOnline(currentMob))
		{
			return false;
		}

		return true;
	}

	public bool IsOnline(GameObject currentMob)
	{
		NetworkConnection connection = currentMob.GetComponent<NetworkIdentity>().connectionToClient;
		if (PlayerList.Instance.ContainsConnection(connection) == false)
		{
			return false;
		}
		return true;
	}
}
