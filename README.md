Library Nuget Package : 
AspNetCore.Identity.MongoDbCore by Alexandra Spieser
MongoDB.Driver by Mongo Inc


///


<Project Sdk="Microsoft.NET.Sdk.Web">

	<PropertyGroup>
		<TargetFramework>net8.0</TargetFramework>
		<Nullable>disable</Nullable>
		<ImplicitUsings>enable</ImplicitUsings>
		<OutputType>WinExe</OutputType>
	</PropertyGroup>

	<ItemGroup>
		<Folder Include="Areas\Admin\Data\" />
		<Folder Include="Areas\Admin\Models\" />
		<Folder Include="wwwroot\images\products\" />
	</ItemGroup>

	<ItemGroup>
		<PackageReference Include="AspNetCore.Identity.MongoDbCore" Version="6.0.0" />
		<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.11" />
		<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.11">
			<PrivateAssets>all</PrivateAssets>
			<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
		</PackageReference>
		<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.7" />
		<PackageReference Include="MongoDB.Driver" Version="2.28.1" />
		<PackageReference Include="MongoDB.Bson" Version="2.28.1" />
	</ItemGroup>

</Project>
