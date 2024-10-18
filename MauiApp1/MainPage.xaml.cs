using MauiApp1.BuildingNETMauiAppDirectSQLServerAccess.Models;
using System;
using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using Dapper;
using MySqlX.XDevAPI.Common;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Data;
using Mysqlx.Crud;
using System.Collections.Generic;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using static Mysqlx.Notice.Warning.Types;
using System.ComponentModel;
using Google.Protobuf.WellKnownTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;

namespace MauiApp1
{   
    public partial class MainPage : ContentPage
    {
        int count = 0;
        DatabaseConnection databaseConnection;

        static string server = "";
        static string database = "";
        static string username = "";
        static string password = "";
        string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

        ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>//////////////// ///<Warning>/// ////////<Warning>//////////////////////////////<Warning>/////////////////////////////<Warning>///////////////////////////////////////////////////////////////////////
        ///<Warning>/////////<Warning>///////////////<Warning>///////////////////////////////<Warning>//////////<Warning>////////////////////////////////////////////////////////////////////////// <Warning>
        /// <Warning>////////<Warning>///////////////////////////////////////////////////<Warning>/////////////////////////<Warning>///////////////////////////////////////////////////////
        /// </Warning>////////////////

        //Used to store the ingredient ID
        List<Ingredients> ingredientsListID = new List<Ingredients>();

        //Used to store the ingredient Name
        List<Ingredients> ingredientsListName = new List<Ingredients>();

        //Used to store the ingredient Quantity
        List<Ingredients> ingredientsListQuantity = new List<Ingredients>();
        public MainPage()
        {
            InitializeComponent();

            //Connect to the Database   -Entry point for the app
            try
            {
                //I am using this as a preventive database error catch ie, it fails before I open any actionsheets or do any processing
                using (var connection = new MySqlConnection(constring))
                {
                    connection.Open();
                    //The database connection is secure and working
                    
                    connection.Close();
                    
                }//End of database connection
            }
            catch (Exception ex)
            {
                //Displays a database fail pop up that will then close the app
                DisplayAlert("Alert", "Connection to the Server has failed", "OK");
                DebugLabel.Text = "Unable to connect to server";
            }
            return;
        }



     


       

        //This method is for when the manage ingredients button is clicked
        //this will call an actionsheet (pre determiend options for the users) the options on the action sheet will be;
        //add ingredient / increase quantity of ingredients // remove ingredient/reduce quantity of ingredients
        private async void ManageIngredientButtonClicked(object sender, EventArgs e)
        {
            //Calls the Manage procedure action sheet
            ManageIngredientActionSheetcall(sender, e);
        }

        //Method that creates and displays an action sheet to the user with the options for adding and editting procedures
        private async void ManageIngredientActionSheetcall(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Select Action:", "Cancel", null, "Add Ingredient", "Increase Quantity", "Decrease Quantity");
            //Action is a string based on what the user selected so ADD Procedure would be a valid action while add Procedure would not be valid
            //Cancel is Cancel in code
            //Clicking off the pop up =   (blank)
            DebugLabel.Text = action;

            //Calls the method to determine what the user just entered
            DetermineActionSheetAction(action);
        }

        //This method will be used to determine which option was selected by the user during an action sheet
        //I will use independent names so that if it was the procedure or ingredient action sheet it does not matter
        private async void DetermineActionSheetAction(string userAction)
        {
            //Switch statement to determine what action was called
            switch (userAction)             //- The Cancel option = Cancel in code
            {
                case "Add Ingredient":
                    //Runs add ingredient datacollection method
                    await AddIngredientItemDataCollection();
                    break;

                case "Increase Quantity":
                    //Runs increase Quantity datacollection method
                    await IncreaseIngredientQuantityDataCollection();
                    break;

                case "Decrease Quantity":
                    //Runs decrease Quantity datacollection method
                    await ReduceIngredientQuantityDataCollection();
                    break;

                case "Add Procedure":
                    //Runs add procedure datacollection method
                    await AddProcedureDataCollection();
                    break;

                case "Edit Procedure":
                    //Runs the edit procedure datacollection method
                    await EditProcedureDataCollection();
                    break;
                case "Cancel":
                    break;
                default:
                    await DisplayAlert("Alert", "Try again", "OK");
                    break;
            }
        }

        //Async functions to manipulate the ingredients on the database

        //Method to collect data for the adding ingredient method
        private async Task<List<Ingredients>> AddIngredientItemDataCollection()
        {
            //ingredientName & ingredientQuantity are both just the inputs from the user ie steak and 7

            //Creates a popup and asks the user to enter the name of the ingredient bought
            string ingredientName = await DisplayPromptAsync("Input", "Please enter the ingredient:");

            int ingredientQuantity = 0;

            bool IngredientMain = false;
            string IngredientMainStringTemp = "";
            string IngredientQuantityTemp = "";

            //Checks if the ingredientName popup recieved input and if it did, asks the user for a quantity otherwise exits the method
            if (!string.IsNullOrEmpty(ingredientName))
            {
                //Creates a popup and asks the user to enter a number for the quantity of ingredients bought
                IngredientQuantityTemp = await DisplayPromptAsync("Input", "Please enter the quantity:");

                //Need to check that quantity is a number otherwise exit (return null)
                if (CheckifValueisInt(IngredientQuantityTemp))
                {
                    //parses the string input into the integer input for the rest of the program
                    int.TryParse(IngredientQuantityTemp, out ingredientQuantity);
                }
                else
                {
                    //The input was not a number and the system has exited
                    await DisplayAlert("Alert", "Quantity was not a number please try again", "OK");
                    return null;
                }

                //Checks if the user entered a quantity
                if (ingredientQuantity > 0)
                {
                    //both ingredient name and ingredient quantity are valid

                    IngredientMainStringTemp = (await DisplayPromptAsync("Input", "Is this ingredient a main? enter 1 for yes 0 for no:"));
                    if (IngredientMainStringTemp == "0")
                    {
                        //The user selected that this ingredient was a side
                        IngredientMain = false;
                    }
                    else if (IngredientMainStringTemp == "1")
                    {
                        //The user selected that this ingredient was a main
                        IngredientMain = true;
                    }
                    else
                    {
                        await DisplayAlert("Alert", "Type of ingredient was invalid please try again", "OK");
                        return null;
                    }

                    await AddIngredientItem(ingredientName, ingredientQuantity, IngredientMain);
                }
            }
            else
            { 
                return null; //Closes the button click method as the user has decided to not select manage ingredients
            }

            return null;
        }

        //Use this method to add an ingredient to the database ie I have bought steak, potatoes, spice mix and jam
        //I would run this method once per ingredient I have bought
        private async Task<List<Ingredients>> AddIngredientItem(string ingredientName, int ingredientQuantity, bool ingredientMain)
        {
            //Checks if the ingredientName exists and checks a quantity was put in //This is kinda redundent but I have it just in case
            if (!string.IsNullOrEmpty(ingredientName) && ingredientQuantity >= 1)
            {
                try
                {
                    
                    //Connects to the database
                    using (var connection = new MySqlConnection(constring))
                    {
                        connection.Open();

                        //Creates parameters username and password are variable names and UserName and Password are the things in the query
                        var checkIngredientExistsQueryParameter = new { ingredientNameParameter = ingredientName, ingredientQuantityParameter = ingredientQuantity };

                        //Creates an sql query to ensure the ingredient doesn't exist before creating it
                        string checkIngredientExistsQueryString = "SELECT IngredientName FROM ingredients WHERE IngredientName = @ingredientNameParameter LIMIT 1;"; 
                        
                        //Runs query
                        var checkIngredientExistsQueryresult = connection.Query(checkIngredientExistsQueryString, checkIngredientExistsQueryParameter);

                        //Check if any results were returned
                        if (checkIngredientExistsQueryresult.Any())
                        {   
                            //for each result in the returned variable
                            foreach (var ingredient in checkIngredientExistsQueryresult)
                            {
                                //There was a result which means the ingredient exists but may not have a quantity
                                await DisplayAlert("Alert", "There is an ingredient with that name or similar please increase its quantity instead", "OK");
                                return null;
                            }
                        }
                        else
                        {   //There was no existing ingredient so here we are creating and inserting into the database a new ingredient
                            //Attempt to create a new item in the database and if not displays an error to the user
                            try
                            {
                                //There was no result which means there was no ingredient with that name
                                //Run insert into query
                                var InsertNewIngredientItemParameter = new { ingredientNameParameter = ingredientName, ingredientQuantityParameter = ingredientQuantity, ingredientMainParamter = ingredientMain };

                                //creates an sql query to insert a new ingredient
                                string InsertNewIngredientItemQuery = "INSERT INTO ingredients (IngredientName, IngredientQuantity, IngredientMain) VALUES (@ingredientNameParameter, @ingredientQuantityParameter, @ingredientMainParamter)";

                                //Runs query to insert new ingredient and quantity and main into the database (does not return anything)
                                var InsertNewIngredientItemQueryresult = connection.Query(InsertNewIngredientItemQuery, InsertNewIngredientItemParameter);

                                //This means the user add an ingredient that didnt exist
                                //It does go into the database
                                await DisplayAlert("Alert", "Ingredient added successfully", "OK");
                            }
                            catch (Exception ex)
                            {
                                //The insert query has failed no ingredient was added to the database
                                await DisplayAlert("Alert", ex.Message, "OK");
                            }
                        }

                        connection.Close();
                        //end of db connection
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log error, show message to user)
                    //DebugLabel.Text = $"Database connection failed: {ex.Message}";
                    DebugLabel.Text = "Error: " + ex.Message;
                }
            }

            //This is where I would call the set ingredients/addingredients method
            return null;
        }




        //Method to collect data to increase the quantity of an already existing ingredient
        private async Task<List<Ingredients>> IncreaseIngredientQuantityDataCollection()
        {
            //Creates a popup and asks the user to enter the name of the ingredient bought
            string ingredientName = await DisplayPromptAsync("Input", "Please enter the ingredient name that you would like to increase the quantity of:");

            int ingredientQuantity = 0;
            string IngredientQuantityTemp = "";

            //Checks if the ingredientName popup recieved input and if it did, asks the user for a quantity otherwise exits the method
            if (!string.IsNullOrEmpty(ingredientName))
            {
                //Creates a popup and asks the user to enter a number for the quantity of ingredients bought
                IngredientQuantityTemp = await DisplayPromptAsync("Input", "Please enter the number you would ADD to the quantity:");

                //Need to check that quantity is a number otherwise exit (return null)
                if (CheckifValueisInt(IngredientQuantityTemp))
                {
                    //parses the string input into the integer input for the rest of the program
                    int.TryParse(IngredientQuantityTemp, out ingredientQuantity);
                }
                else
                {
                    //The input was not a number and the system has exited
                    await DisplayAlert("Alert", "Quantity was not a number please try again", "OK");
                    return null;
                }

                //Checks if the user entered a quantity
                if (ingredientQuantity >= 1)
                {
                    //both ingredient name and ingredient quantity are valid
                    await IncreaseIngredientQuantity(ingredientName, ingredientQuantity);
                }
            }
            else
            {
                return null; //Closes the button click method as the user has decided to not select manage ingredients
            }

            return null;
        }



        //Use this method to increase the quantity of an ingredient I have, ie I have 2 days of steaks but I have bought more
        //This method would increase the number of dinner sized steak portions I have
        private async Task<List<Ingredients>> IncreaseIngredientQuantity(string ingredientName, int ingredientQuantity)
        {
            try
            {
                using (var connection = new MySqlConnection(constring))
                {

                    //Creates parameters username and password are variable names and UserName and Password are the things in the query
                    var checkIngredientExistsQueryParameter = new { ingredientNameParameter = ingredientName, ingredientQuantityParameter = ingredientQuantity };

                    //Creates an sql query to ensure the ingredient doesn't exist before creating it
                    string checkIngredientExistsQueryString = "SELECT IngredientName FROM ingredients WHERE IngredientName = @ingredientNameParameter LIMIT 1;";

                    //Runs query
                    var checkIngredientExistsQueryresult = connection.Query(checkIngredientExistsQueryString, checkIngredientExistsQueryParameter);

                    if (checkIngredientExistsQueryresult.Any())
                    {
                        //there was an ingredient named that; we dont need to check that quantity makes sense as during the data collection phase this is sorted

                        //Creates query to update the correct ingredient with a new quantity
                        string UpdateIngredientQuantityQueryString = "update ingredients SET IngredientQuantity = (SELECT IngredientQuantity WHERE IngredientName = @ingredientNameParameter) + @ingredientQuantityParameter WHERE IngredientName = @ingredientNameParameter AND IngredientID = (SELECT IngredientID WHERE IngredientName = @ingredientNameParameter);";

                        //Runs query
                        var UpdateIngredientQuantityQueryResult = connection.Query(UpdateIngredientQuantityQueryString, checkIngredientExistsQueryParameter);

                        //Show the user a success message
                        await DisplayAlert("Alert", "Quantity increased successfully", "OK");
                    }
                    else
                    {
                        //There was no ingredient named that please leave
                        await DisplayAlert("Alert", "There is no igredient with that name", "OK");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
            return null;
        }





        //Method to collect data to decrease the quantity of an already existing ingredient
        private async Task<List<Ingredients>> ReduceIngredientQuantityDataCollection()
        {
            //Creates a popup and asks the user to enter the name of the ingredient bought
            string ingredientName = await DisplayPromptAsync("Input", "Please enter the ingredient name that you would like to reduce the quantity of:");

            int ingredientQuantity = 0;
            string IngredientQuantityTemp = "";

            //Checks if the ingredientName popup recieved input and if it did, asks the user for a quantity otherwise exits the method
            if (!string.IsNullOrEmpty(ingredientName))
            {
                //Creates a popup and asks the user to enter a number for the quantity of ingredients bought
                IngredientQuantityTemp = await DisplayPromptAsync("Input", "Please enter the number you would SUBTRACT from the quantity:");

                //Need to check that quantity is a number otherwise exit (return null)
                if (CheckifValueisInt(IngredientQuantityTemp))
                {
                    //parses the string input into the integer input for the rest of the program
                    int.TryParse(IngredientQuantityTemp, out ingredientQuantity);
                }
                else
                {
                    //The input was not a number and the system has exited
                    await DisplayAlert("Alert", "Quantity was not a number please try again", "OK");
                    return null;
                }

                //Checks if the user entered a quantity
                if (ingredientQuantity > 0)
                {
                    //both ingredient name and ingredient quantity are valid
                    await ReduceIngredientQuantity(ingredientName, ingredientQuantity);
                }
            }
            else
            {
                return null; //Closes the button click method as the user has decided to not select manage ingredients
            }

            return null;
        }

        //Use this method to reduce the quantity of an ingredient I have, ie I have 2 days of steaks but I have used them
        //This method would reduce the number of dinner sized steak portions I have

        private async Task<List<Ingredients>> ReduceIngredientQuantity(string ingredientName, int ingredientQuantity)
        {
            try
            {
                using (var connection = new MySqlConnection(constring))
                {

                    //Creates parameters username and password are variable names and UserName and Password are the things in the query
                    var checkIngredientExistsQueryParameter = new { ingredientNameParameter = ingredientName, ingredientQuantityParameter = ingredientQuantity };

                    //Creates an sql query to ensure the ingredient doesn't exist before creating it
                    string checkIngredientExistsQueryString = "SELECT IngredientName FROM ingredients WHERE IngredientName = @ingredientNameParameter LIMIT 1;";

                    //Runs query
                    var checkIngredientExistsQueryresult = connection.Query(checkIngredientExistsQueryString, checkIngredientExistsQueryParameter);

                    if (checkIngredientExistsQueryresult.Any())
                    {
                        //there was an ingredient named that; we dont need to check that quantity makes sense as during the data collection phase this is sorted

                        //Creates query to update the correct ingredient with a new quantity
                        string UpdateIngredientQuantityQueryString = "update ingredients SET IngredientQuantity = (SELECT IngredientQuantity WHERE IngredientName = @ingredientNameParameter) - @ingredientQuantityParameter WHERE IngredientName = @ingredientNameParameter AND IngredientID = (SELECT IngredientID WHERE IngredientName = @ingredientNameParameter);";

                        //Runs query
                        var UpdateIngredientQuantityQueryResult = connection.Query(UpdateIngredientQuantityQueryString, checkIngredientExistsQueryParameter);

                        //Show the user a success message
                        await DisplayAlert("Alert", "Quantity reduced successfully", "OK");
                    }
                    else
                    {
                        //There was no ingredient named that please leave
                        await DisplayAlert("Alert", "There is no igredient with that name", "OK");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
            return null;
        }

        //Use this method to remove an Ingredient from the Database, ie I have used all of my steak, potatoes on a meal






        //This method is for when the manage procedures button is clicked
        private async void ManageProcedureButtonClicked(object sender, EventArgs e)
        {
            //Calls the Manage procedure action sheet
            ManageProcedureActionsheetcall(sender, e);
        }

        //Method that creates and displays an action sheet to the user with the options for adding and editting procedures
        async void ManageProcedureActionsheetcall(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Select Action:", "Cancel", null, "Add Procedure", "Edit Procedure");
            //Action is a string based on what the user selected so ADD Procedure would be a valid action while add Procedure would not be valid
            
            //Calls the method to determine what the user just entered
            DetermineActionSheetAction(action);
        }


        //Method to add a procedure to the database and perform checks to ensure validity

        //method to collect data for the adding procedure method
        async Task<List<Ingredients>> AddProcedureDataCollection()
        {
            //Creates a popup and asks the user to enter the name of the ingredient bought
            string ingredientName = await DisplayPromptAsync("Input", "Please enter the name of the ingredient you would like to add the procedure for:");

            string IngredientProcedure = "";
            string IngredientProcedureName = "";

            //Checks if the ingredientName popup recieved input and if it did, asks the user for a quantity otherwise exits the method
            if (!string.IsNullOrEmpty(ingredientName))
            {
                //Creates a popup and asks the user to enter a number for the quantity of ingredients bought
                IngredientProcedure = await DisplayPromptAsync("Input", "Please enter the procedure description you would like to add:");

                //Need to check that something was entered for a procedure
                if (!string.IsNullOrEmpty(IngredientProcedure))
                {
                    IngredientProcedureName = await DisplayPromptAsync("Input", "Please enter the name of the procedure you just added:");
                    if (!string.IsNullOrEmpty(IngredientProcedure))
                    {

                        await AddProcedure(ingredientName, IngredientProcedure, IngredientProcedureName);
                    }
                    else
                    {
                        //The input was not a number and the system has exited
                        await DisplayAlert("Alert", "Nothing was entered for the procedure name", "OK");
                        return null;
                    }
                }
                else
                {
                    //The input was not a number and the system has exited
                    await DisplayAlert("Alert", "Nothing was entered for the procedure", "OK");
                    return null;
                }
            }
            else
            {
                await DisplayAlert("Alert", "That ingredient does not exist", "OK");
                return null;
            }
            return null;

        }
        //Method to add a procedure to the database
        async Task<List<Ingredients>> AddProcedure(string ingredientName, string procedureDescription, string procedureName)
        {
            try
            {
                using (var connection = new MySqlConnection(constring))
                {

                    //Creates parameters username and password are variable names and UserName and Password are the things in the query
                    var checkIngredientExistsQueryParameter = new { ingredientNameParameter = ingredientName, procedureDescriptionParameter = procedureDescription };

                    //Creates an sql query to ensure the ingredient doesn't exist before creating it
                    string checkIngredientExistsQueryString = "SELECT IngredientName FROM ingredients WHERE IngredientName = @ingredientNameParameter LIMIT 1;";

                    //Runs query
                    var checkIngredientExistsQueryresult = connection.Query(checkIngredientExistsQueryString, checkIngredientExistsQueryParameter);

                    if (checkIngredientExistsQueryresult.Any())
                    {
                        try
                        {
                            //This means there was a result returned = the ingredient entered by the user does exist

                            var addProcedureQueryParameter = new { ingredientNameParameter = ingredientName, procedureDescriptionParameter = procedureDescription , procedureNameParameter = procedureName };

                            //Creates an sql query to insert the new procedure into the database
                            string addProcedureQueryString = "INSERT INTO procedures (ProcedureName,ProcedureDescription) VALUES (@procedureNameParameter, @procedureDescriptionParameter)";

                            //Runs query
                            var addProcedureQueryresult = connection.Query(addProcedureQueryString, addProcedureQueryParameter);

                            //Adding procedure was successful
                            await DisplayAlert("Alert", "Procedure added successfully", "OK");


                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Alert", ex.Message, "OK");
                            return null;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "An ingredient with that name does not exist", "OK");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
            return null;

        }




        //Method to collect data for the edit procedure method
        async Task<List<Ingredients>> EditProcedureDataCollection()
        {
            //Creates a popup and asks the user to enter the name of the ingredient bought
            string procedureName = await DisplayPromptAsync("Input", "Please enter the name of the procedure you would like to edit:");
            string ProcedureNewDescription = "";

            //Checks if something was entered into the procedureName box
            if (!string.IsNullOrEmpty(procedureName))
            {
                //Creates a popup and asks the user to enter a number for the quantity of ingredients bought
                ProcedureNewDescription = await DisplayPromptAsync("Input", "Please enter the new procedure description:");

                //Checks if something was entered into the procedureDescription box
                if (!string.IsNullOrEmpty(ProcedureNewDescription))
                {
                    await EditProcedure(procedureName, ProcedureNewDescription);
                }
                else
                {
                    //The input was not a number and the system has exited
                    await DisplayAlert("Alert", "Nothing was entered for the procedure description", "OK");
                    return null;
                }
            }
            else
            {
                await DisplayAlert("Alert", "No procedure entered", "OK");
                return null;
            }
            return null;
        }

        //Method to edit a procedure already in the database and perform validitity checks
        async Task<List<Ingredients>> EditProcedure(string procedureName, string procedureNewDescription)
        {
            try
            {
                using (var connection = new MySqlConnection(constring))
                {

                    //Creates parameters username and password are variable names and UserName and Password are the things in the query
                    var checkProceduerExistsQueryParameter = new { procedureNameParameter = procedureName, procedureDescriptionParameter = procedureNewDescription };

                    //Creates an sql query to ensure the ingredient doesn't exist before creating it
                    string checkProcedureExistsQueryString = "SELECT ProcedureName FROM procedures WHERE ProcedureName = @procedureNameParameter LIMIT 1;";

                    //Runs query
                    var checkProcedureExistsQueryResult = connection.Query(checkProcedureExistsQueryString, checkProceduerExistsQueryParameter);

                    //if anything was returned from the query (that procedure exists)
                    if (checkProcedureExistsQueryResult.Any())
                    {
                        try
                        {
                            //This means there was a result returned = the ingredient entered by the user does exist

                            var editProcedureQueryParameter = new {procedureDescriptionParameter = procedureNewDescription, procedureNameParameter = procedureName };

                            //Creates an sql query to insert the new procedure into the database
                            string editProcedureQueryString = "UPDATE procedures SET ProcedureDescription = @procedureDescriptionParameter WHERE ProcedureName = @procedureNameParameter";

                            //Runs query
                            connection.Query(editProcedureQueryString, editProcedureQueryParameter);

                            //Adding procedure was successful
                            await DisplayAlert("Alert", "Procedure changed successfully", "OK");
                            //No need for an if as the try catch statement should catch any issues

                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Alert", ex.Message, "OK");
                            return null;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Alert", "A procedure does not exist with that name", "OK");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alert", ex.Message, "OK");
            }
            return null;

    }


        private bool CheckifValueisInt(string quantity)
        {
            //a is the output
            int a;
            if (int.TryParse(quantity, out a))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //This method will be used to generate a meal plan and display it into a
        async Task<List<Ingredients>> GenerateMeal()
        {
            return null;
        }









        /// <summary>
        /// //////////////////////////////////////////////////////
        /// </summary>
        /// <param ></param>
        /// <param></param>
        /// 



        //Default button clicked event
        private void Button_Clicked(object sender, EventArgs e)
        {
            // Implement button click functionality here
            DebugLabel.Text = "Button clicked";
        }

        //old non async versions of the Ingredient DB functions


        //Use this method to add an ingredient to the database ie I have bought steak, potatoes, spice mix and jam
        //I would run this method once per ingredient I have bought
        public void AddIngredientItem(object sender, EventArgs e)
        {
            //Opens database connection
            using (var connection = new MySqlConnection(constring))
            {
                connection.Open();




                var AddIngredientItemQuery = "INSERT INTO ingredients (IngredientName,Quantity) VALUES ()";




                connection.Close();
            }
        }

        //Use this method to increase the quantity of an ingredient I have, ie I have 2 days of steaks but I have bought more
        //This method would increase the number of dinner sized steak portions I have
        public void IncreaseIngredientQuantity(object sender, EventArgs e)
        {
            //Opens database connection
            using (var connection = new MySqlConnection(constring))
            {
                connection.Open();
                connection.Close();
            }
        }

        //Use this method to remove an Ingredient from the Database
        //ie I have used all of my steak, potatoes on a meal
        public void RemoveIngredientItem(object sender, EventArgs e)
        {
            //Opens database connection
            using (var connection = new MySqlConnection(constring))
            {
                connection.Open();
                connection.Close();
            }
        }

        //Use this method to reduce the quantity of an ingredient I have, ie I have 2 days of steaks but I have used them
        //This method would reduce the number of dinner sized steak portions I have
        public void ReduceIngredientQuantity(object sender, EventArgs e)
        {
            //Opens database connection
            using (var connection = new MySqlConnection(constring))
            {
                connection.Open();
                connection.Close();
            }
        }

        //No idea if its used just left
        public async Task<List<Contact>> AsyncTest()
        {
            List<Contact> ret = null;
            string result = await DisplayPromptAsync("Question 2", "What's 5 + 5?", initialValue: "10", maxLength: 2, keyboard: Keyboard.Numeric);
            return ret;
        }


    }
}
