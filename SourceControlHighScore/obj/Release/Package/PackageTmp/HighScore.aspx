<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighScore.aspx.cs" Inherits="SourceControlHighScore.HighScore" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Williams Lea Development Highscore Table</title>
    <script type="text/javascript" src="js/jquery-1.11.1.js"></script>
    <script type="text/javascript" src="js/HighScore.js"></script>
    <link rel="stylesheet" type="text/css" href="css/HighScore.css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div id="hsTitle">
                Development High Score
            </div>
            <div id="gameOver">
                INSERT COIN
            </div>
            <div id="hsTable">

                <div id="titles">
                    <div id="titlecontent">
                        <p style="color:white">What follows is a list of names that have files checked out in TFS</p>
                        <asp:Literal runat="server" ID="NamesList"></asp:Literal>
                    </div>
                </div>
            </div>​​​​​​​​​
        </div>
    </form>
</body>
</html>
