using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class DB_Manager : MonoBehaviour
{
    public static DB_Manager instance;
    public string host,database,username,password;
    MySqlConnection con;

    private void Start()
    {
        Connect_BDD();

        string strOutput = LeaderBoard(10);
        Debug.Log("DBtest: " + strOutput);
    }
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Connect_BDD()
    {
        String cmd = "SERVER=" + host + ";" + "database =" + database + ";User ID=" + username + ";Password=" + password + ";Pooling=true;Charset=utf8;" ;
        try
        {
            con = new MySqlConnection(cmd);
            con.Open();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    private void Update()
    {
        //Debug.Log(con.State);
    }

    public string LeaderBoard(int limit)
    {
        try
        {
            //Connect_BDD();
            MySqlCommand cmdSql = new MySqlCommand("SELECT * FROM `Tiles` ORDER BY `Consommation 12 derniers mois` DESC LIMIT " + limit, con);
            MySqlDataReader myReader = cmdSql.ExecuteReader();

            string data = null;
            while (myReader.Read())
            {
                data += myReader["Code Art"].ToString() + ": " + myReader["Consommation 12 derniers mois"] + "\n";
            }
            myReader.Close();
            return data;
        }
        catch
        {
            return null;
        }
    }

}
