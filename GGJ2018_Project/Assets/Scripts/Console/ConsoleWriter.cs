﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleWriter : MonoBehaviour
{
	public delegate void SendCommand(string cmd, string[] args);
	private event SendCommand OnSendCommand;

	public delegate void ErrorCommand(string cmd);
	private event ErrorCommand OnErrorCommand;

	[SerializeField]
	private GameObject console;
	[SerializeField]
	private InputField writer;

	void Start()
	{
		writer.onEndEdit.AddListener(SendConsole);
		AddOnSendCommand(DebugCommand);
		ShowConsole(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			ShowConsole(true);
		}
	}

	private void SendConsole(string cmdLine)
	{
		string[] parseCommand = cmdLine.Split(' ');

		string cmd = parseCommand.Length > 0 ? parseCommand[0] : "";

		string[] args = new string[] { };
		if (parseCommand.Length > 1)
		{
			args = new string[parseCommand.Length - 1];

			for (int i = 1 ; i < parseCommand.Length ; ++i)
			{
				args[i - 1] = parseCommand[i];
			}
		}

		InvokeOnSendCommand(cmd, args);

		ShowConsole(false);
	}

	private void ShowConsole(bool b)
	{
		console.SetActive(b);
		writer.text = "";
		enabled = !console.activeSelf;
		writer.Select();
	}

	private void DebugCommand(string cmd, string[] args)
	{
		Debug.Log("cmd : " + cmd);
		foreach (string arg in args)
		{
			Debug.Log("\t arg : " + arg);
		}
	}

	#region Events
	#region OnSendCommand
	public void AddOnSendCommand(SendCommand func)
	{
		OnSendCommand += func;
	}

	public void RemoveOnSendCommand(SendCommand func)
	{
		OnSendCommand -= func;
	}

	private void ResetOnSendCommand(SendCommand func)
	{
		OnSendCommand = null;
	}

	private void InvokeOnSendCommand(string cmd, string[] args)
	{
		if (OnSendCommand != null)
			OnSendCommand(cmd, args);
	}
	#endregion

	#region OnSendCommand
	public void AddOnErrorCommand(ErrorCommand func)
	{
		OnErrorCommand += func;
	}

	public void RemoveOnErrorCommand(ErrorCommand func)
	{
		OnErrorCommand -= func;
	}

	private void ResetOnErrorCommand(ErrorCommand func)
	{
		OnErrorCommand = null;
	}

	public void InvokeOnErrorCommand(string cmd)
	{
		if (OnErrorCommand != null)
			OnErrorCommand(cmd);
	}
	#endregion
	#endregion

}
