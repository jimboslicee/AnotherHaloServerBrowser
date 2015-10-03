# AnotherHaloServerBrowser
Using the unofficical Halo Online game, this server browser displays all servers and player stats in ASP.NET and SignalR

Viewable on [dew.jimmyli.co](http://dew.jimmyli.co)

This is a prototype. The code structure is a bit of a mess and I have plans to improve ...well everything. I plan on rebuilding it with an API, a better UI, and definitely a better persistence model (either redis or memcached) rather than the built-in MemoryCache (it's still good, but there are better ones). 

Hubs and worker timer threads will be distinctly defined for each operation:

* Fetching the master server list
* Adding each server to the cache
* Sending each server info to the clients from the cache
* Handling each client's current view on a server (they can select a server and view its info, rather than show all of it)

Of course, don't hesitate to write some issues or leave feedback.

-Jimbo