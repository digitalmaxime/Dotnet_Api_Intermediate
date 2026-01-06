## Chat AI Agent

Rest API for chatting with an AI agent. 

### JWT Tokens

using Jwt Bearer Authentication to know who the user is.

use dotnet tool : `dotnet user-secrets --claim name=maxou` 
in order to set the claim.

Then pass that bearer token in the header of the POST request.