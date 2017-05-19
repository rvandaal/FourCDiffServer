# FourCDiffServer
Exercise project

Open this readme in raw view
1. Run the FourCDiffServer from within Visual Studio 2015. This will open up the browser.
2. Open the tool Fiddler, go to the Composer tab, subtab Scratchpad and paste the following text (replace the portnumber by the right portnumber).

==========================================================
PUT http://localhost:19089/v1/diff/1/Left HTTP/1.1
User-Agent: Fiddler
Host: localhost:19089
Content-Type: application/json; charset=utf-8
Content-Length: 21

{"Data":"Helloworld"}
==========================================================

PUT http://localhost:19089/v1/diff/1/Right HTTP/1.1
User-Agent: Fiddler
Host: localhost:19089
Content-Type: application/json; charset=utf-8
Content-Length: 21

{"Data":"Helkavorlt"}
==========================================================

GET http://localhost:19089/v1/diff/1 HTTP/1.1
User-Agent: Fiddler
Host: localhost:19089

3. Select the GET request (3 lines) and press 'Execute' in the topright corner.
4. You will receive a 404 Not Found response
5. Execute the two PUT requests one by one (including the JSON)
6. For each PUT request, you will receive a 201 Created response
7. Execute the GET request again
8. You will receive a 200 OK response with the diff of the two JSON inputs, also in JSON format (check 'TextView' for the content).
