# GraphQL (Advanced)
* Connect to db
* Install packages

### Connect to db
Connect to db from section 3
### Install packages
Install packages `HotChocolate.AspNetCore` and `HotChocolate.Data.EntityFramework` and `GraphQL.Server.Ui.Voyager`
```csproj
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="GraphQL.Server.Ui.Voyager" Version="5.0.0" />
    <PackageReference Include="HotChocolate.AspNetCore" Version="11.1.0" />
    <PackageReference Include="HotChocolate.Data.EntityFramework" Version="11.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.0" NoWarn="NU1605" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="5.0.0" NoWarn="NU1605" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="5.6.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="5.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="5.0.0" />
  </ItemGroup>
</Project>
```
