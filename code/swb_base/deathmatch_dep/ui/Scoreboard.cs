
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Scoreboard : Sandbox.UI.Scoreboard<ScoreboardEntry>
{

    public Scoreboard()
    {
		this.StyleSheet.Load("swb_base/deathmatch_dep/ui/scss/Scoreboard.scss");
    }

    protected override void AddHeader()
    {
		this.Header = this.Add.Panel("header");
		this.Header.Add.Label("player", "name");
		this.Header.Add.Label("kills", "kills");
		this.Header.Add.Label("deaths", "deaths");
		this.Header.Add.Label("ping", "ping");
    }
}

public class ScoreboardEntry : Sandbox.UI.ScoreboardEntry
{
}
