﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RiskLib;
using System.Drawing;

namespace Risk
{    
    public partial class LoadBoard : System.Web.UI.Page
    {
        RiskGame Game 
        {
            /// Store game in session so it retains its object instance

            get { return (RiskGame)Session["Game"]; }
            set { Session["Game"] = value; }
        }
        
        // Dynamically created controls
        Dictionary<string, LinkButton> TerritoryLinks;
        Dictionary<string, Label> PlayerNameLabels;
        Dictionary<string, Label> TroopLabels;


        protected void Page_Init(object sender, EventArgs e)
        {   
            /// Create the board controls at runtime
            
            CreateBoard();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /// Add events to controls created at runtime
            
            foreach (LinkButton lb in TerritoryLinks.Values)
            {
                lb.Click += TerritoryClick;
            }
        }

        protected void TerritoryClick(object sender, EventArgs e)
        {
            LinkButton lb = (LinkButton)sender;
            Game.TerritorySelected(lb.CommandArgument);

            UpdateLabels();
        }

        #region <UI methods>

        private void CreateBoard()
        {
            TerritoryLinks = new Dictionary<string, LinkButton>();
            PlayerNameLabels = new Dictionary<string, Label>();
            TroopLabels = new Dictionary<string, Label>();

            RiskBoard EmptyBoard = new RiskBoard(Server.MapPath("Risk.xml"));

            Table T = new Table();
            //if (Game == null) T.Visible = false;

            foreach (BoardTerritory t in EmptyBoard.Territories
                                            .OrderBy(n => n.Name)
                                            .OrderBy(n => n.Continent.Name))
            {
                /*
                TableRow tr = new TableRow();
                T.Rows.Add(tr);

                TableCell ContinentCell = new TableCell();
                TableCell TerritoryNameCell = new TableCell();
                TableCell PlayerNameCell = new TableCell();
                TableCell TroopsCell = new TableCell();
                TableCell NeighborsCell = new TableCell();
                tr.Cells.Add(ContinentCell);
                tr.Cells.Add(TerritoryNameCell);
                tr.Cells.Add(PlayerNameCell);
                tr.Cells.Add(TroopsCell);
                tr.Cells.Add(NeighborsCell);

                Label clabel = new Label();
                clabel.Text = t.Continent.Name;
                ContinentCell.Controls.Add(clabel);

                Label nlabel = new Label();
                nlabel.Text = t.AdjacentTerritories.Count.ToString();
                NeighborsCell.Controls.Add(nlabel);
                */


                LinkButton lb = new LinkButton();
                lb.ID = t.Name;
                lb.CommandArgument = t.Name;
                lb.Text = "0";
                lb.CssClass = GetCssClass(t.Name);
                lb.ToolTip = t.Name;
                
                //TerritoryNameCell.Controls.Add(lb);
                //Label nameLabel = new Label();
                //PlayerNameCell.Controls.Add(nameLabel);
                //Label troopsLabel = new Label();
                //TroopsCell.Controls.Add(troopsLabel);
                //PlayerNameLabels.Add(t.Name, nameLabel);
                //TroopLabels.Add(t.Name, troopsLabel);

                TerritoryLinks.Add(t.Name, lb);
                PlaceHolder2.Controls.Add(lb);
            }

            //PlaceHolder1.Controls.Add(T);
        }

        private void UpdateLabels()
        {
            StateLabel.Text = Game.State.ToString();
            TurnStateLabel.Text = Game.TurnState();
            PlayersLabel.Text = Game.PlayersAsList();

            //if (Game.TurnInProgress)
            //{
            //    foreach (LinkButton lb in TerritoryLinks.Values)
            //    {
            //        if (Game.CurrentPlayer.Territories.Where(t => t.boardTerritory.Name == lb.Text).Count() == 0)
            //            lb.Enabled = false;
            //        else
            //            lb.Enabled = true;
            //    }
            //}

            // 'Game Board' Labels

            foreach (BoardTerritory t in Game.Board.Territories)
            {
                try
                {
                    PlayerTerritory pt = Game.PlayerTerritories.Where(x => x.boardTerritory.Name == t.Name).Single();
                    TerritoryLinks[t.Name].CssClass = GetCssClass(t.Name);
                    //PlayerNameLabels[t.Name].Text = pt.Player.Name;
                    TerritoryLinks[t.Name].Text = pt.Troops.ToString();
                    TerritoryLinks[t.Name].ToolTip += " - " + pt.Player.Name;
                    TerritoryLinks[t.Name].BackColor = pt.Player.color;
                }
                catch
                {
                    TerritoryLinks[t.Name].Text = "0";
                    TerritoryLinks[t.Name].CssClass = GetCssClass(t.Name) + " empty";
                    //TroopLabels[t.Name].Text = "0";
                    //((TableRow)PlayerNameLabels[t.Name].Parent.Parent).BackColor = Color.White;
                }
            }
        }

        private string GetCssClass(string name)
        {
            return name.ToLower().Replace(' ', '_');
        }

        #endregion


        #region  <game setup, simple calls to the RiskGame object>

        protected void NewGame(object sender, EventArgs e)
        {
            Game = new RiskGame(Server.MapPath("Risk.xml"));

            UpdateLabels();
        }

        protected void AddPlayer(object sender, EventArgs e)
        {
            TextBox t = (TextBox)UpdatePanel1.FindControl("TextBox1");
            Game.AddPlayer(t.Text);

            UpdateLabels();
        }

        protected void AssignTerritories(object sender, EventArgs e)
        {
            Game.AssignTerritoriesRandomly(new Random(DateTime.Now.Second));

            UpdateLabels();
        }

        protected void EndAttack(object sender, EventArgs e)
        {
            Game.EndAttack();

            UpdateLabels();
        }

        #endregion

    }
}