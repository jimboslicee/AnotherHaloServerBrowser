# AnotherHaloServerBrowser
Hosted on AWS and using the unoffical Halo Online game, this server browser displays all servers and player stats in ASP.NET and SignalR.

**NOTE: THERE MAY BE OFFENSIVE NAMES AND WORDS ON THIS WEB APP.**

**I AM NOT RESPONSIBLE FOR THEIR CONTENT AS IT IS USER GENERATED**

Project is live on [dew.jimmyli.co](http://dew.jimmyli.co)


# How It Works

I wanted to play with SignalR and this project idea came from that desire. It is used to delegate all updates on the backend rather than the client polling at an interval. It makes sense because the client will not be acting as an active user requesting data from an API, rather they are a passive user taking in all the info the server gives them.

The community devs from the unofficial Halo Online game have a backend that act as a master server list containing all the host's servers. 

This application will ping that master server list for a JSON response every 30 seconds (to avoid flooding) and update the server list held in cache. Servers are also removed when differences are compared. (A foreseeable issue is that a closed server will continue to be present for another 30 seconds)

Each JSON response is a string array of IP addresses of servers being hosted.

Example (these should not be real host IP addresses, any actual connection made is a coincidence):
```
	{"result": 
		{
			"msg": "OK", 
			"code": 0, 
			"servers": ["111.222.133.144:11775", "99.98.97.96:11775", "1.2.3.4:11775", "192.168.1.1:11775", "172.168.16.5:11775"]
		}, 
		"listVersion": 1
	}
```
Every second another background worker will ping each and every server for another JSON response that contains the server info, such as:
```
	{
	  "name": "HaloOnline Server",
	  "port": 11774,
	  "hostPlayer": "Player 1",
	  "sprintEnabled": "1",
	  "sprintUnlimitedEnabled": "0",
	  "VoIP": true,
	  "teams": false,
	  "map": "Guardian",
	  "mapFile": "guardian",
	  "variant": "Sword Mode",
	  "variantType": "slayer",
	  "status": "InGame",
	  "numPlayers": 12,
	  "maxPlayers": 16,
	  "xnkid": "00000000000000000000000000000000",
	  "xnaddr": "00000000000000000000000000000000",
	  "players": [
	    {
	      "name": "Player 1",
	      "score": 9,
	      "kills": 9,
	      "assists": 0,
	      "deaths": 4,
	      "team": 0,
	      "isAlive": true,
	      "uid": "00000000000000"
	    },
	    {
	      "name": "Player 2",
	      "score": 3,
	      "kills": 3,
	      "assists": 0,
	      "deaths": 6,
	      "team": 1,
	      "isAlive": false,
	      "uid": "00000000000000"
	    },
	    {
	      "name": "Player 3",
	      "score": 0,
	      "kills": 0,
	      "assists": 0,
	      "deaths": 2,
	      "team": 2,
	      "isAlive": true,
	      "uid": "000000000000000"
	    },
	    ...etc
	  ],
	  "gameVersion": "1.106708_cert_ms23___release",
	  "eldewritoVersion": "0.4.11.2"
	}

```

This is then saved into cache (server ip:json response key value pair) and every second another timer will update clients on each server with that info.

# Plans

This is a prototype. The code structure is a bit of a mess and I have plans to improve ...well everything. I plan on rebuilding it with an API (sorted server list), a better UI, non-hardcoded master server URLs, and definitely a better persistence model (either redis or memcached) rather than the built-in MemoryCache (it's still good, but there are better ones). 

Hubs and timer threads will be distinctly defined for each operation:

* Fetching the master server list
* Adding each server to the cache
* Sending each server info to the clients from the cache
* Handling each client's current view on a server (they can select a server and view its info, rather than show all of it)

Of course, don't hesitate to write some issues or leave feedback.

-Jimbo