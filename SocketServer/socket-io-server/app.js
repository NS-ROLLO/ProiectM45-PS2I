//**************************************************************************************************/
/**********Below is the code for creating Web Socket Connection for communication with ReactApp*****/
//**************************************************************************************************/

//REQUIRES!
const express = require("express");
const http = require("http");
const socketIo = require("socket.io");
const axios = require("axios");
const index = require("./routes/index");

var isReactAppConnected = false;
//reactAppSocket is the WebSocket used to transfer data between ReactApp and Node.js server
var reactAppSocket;
//web socket port for communication with ReactApp
const port = process.env.PORT || 5000;

const app = express();
app.use(index);

const server = http.createServer(app);

const io = socketIo(server);


io.on("connection", socket => {
  console.log("Client connected!");
  reactAppSocket = socket;
  isReactAppConnected = true;
  reactAppSocket.on("disconnect", () => console.log("Client disconnected!"));
});

///code for sending data from Node.js server at a set interval to the ReactApp
// io.on("connection", socket => {
//     console.log("New client connected"), setInterval(
//       () => emitDataToApp(socket),
//       10000
//     );
//     socket.on("disconnect", () => console.log("Client disconnected"));
//   });

//server starts listening
server.listen(port, () => console.log(`Listening on port ${port}`));  

//method can be used as an example to send data to the ReactApp after fetching it from an external API
const emitDataToApp = async socket => {
    try {      
      const res = Math.floor(Math.random() * Math.floor(50));
      ///code commented can be used to call external API and send data to the ReactApp
      //const res = await axios.get(
      //"https://api.darksky.net/forecast/PUT_YOUR_API_KEY_HERE/43.7695,11.2558"
      //); // Getting the data from DarkSky
      //socket.emit("FromAPI", res.data.currently.temperature); // Emitting a new message. It will be consumed by the client
      socket.emit("FromAPI", res); // Emitting a new message. It will be consumed by the client
    } catch (error) {
      console.error(`Error: ${error.code}`);
    }
  };  

//#region TCP Socket Communication with PLC Simulator


//**************************************************************************************************/
/**********Below is the code for creating TCP Connection for communication with PLC Simulator*******/
//**************************************************************************************************/

var net = require('net');
//TCP Socket for the connection with the Simulator
//HOST (IP): localhost (127.0.0.1)
//PORT: port used for creating the socket communication
var HOST = '127.0.0.1';
var PORT = 5500;

// Create a server instance, and chain the listen function to it
// The function passed to net.createServer() becomes the event handler for the 'connection' event
// The sock object the callback function receives UNIQUE for each connection
net.createServer(function(sock) {
//We have a connection - a socket object is assigned to the connection automatically
 console.log('CONNECTED: ' + sock.remoteAddress +':'+ sock.remotePort);
  //Add a 'data' event handler to this instance of socket
  //data[] is the 
  sock.on('data', function(data) {    
    console.log('DATA ' + sock.remoteAddress + ': ' + sock.bytesRead + ' ---> data read:  '  + data[0].toString() + ' ' + data[1].toString());
    if (isReactAppConnected === true){
      reactAppSocket.emit("FromAPI", data[0]); // Emitting a new message. It will be consumed by the client
    }
    // Write the data back to the socket, the client will receive it as data from the server
    //sock.write('You said "' + data + '"');
  });
  // Add a 'close' event handler to this instance of socket
 sock.on('close', function(data) {
   console.log('CLOSED: ' + sock.remoteAddress +' '+ sock.remotePort);
 });

}).listen(PORT, HOST);

console.log('Server listening on ' + HOST +':'+ PORT);

//#endregion TCP Socket Communication with PLC Simulator

