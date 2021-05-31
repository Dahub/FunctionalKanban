migration command
power shell command line

Event DB :  
	dotnet ef migrations add <name> --context EventDbContext -o EventDatabase\Migrations\
	dotnet ef database update --context EventDbContext

ViewProjection DB :
	dotnet ef migrations add <name> --context ViewProjectionDbContext -o ViewProjectionDatabase\Migrations\
	dotnet ef database update --context ViewProjectionDbContext