# FundusSegmentation4Android

To run the server and the client, you have to build a wireless LAN using your computer's wireless card.
You shouldn't use the school Internet environment or the router at home, since they will allocate the server computer and the android phone into two different subnet.

After you have successfully build up a Wireless LAN, you should check the IP address of the computer that host the server program.
Then change the code of both server and the android client.

At the server side, you can check the main cpp file and the the beginning, there is a setting of socket IP address. You should change the previous IP address to the new one you just check.

At the client side, you should goto the ImageProcessing.cs and at the function called StartClient, change the setting IP Address to the new IPI address you just get.

Then Both devices can communicate with each other.
