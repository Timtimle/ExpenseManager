# Auto-generate PlantUML diagram from C# project
param(
    [string]$ProjectPath = "E:\Projects\ExpenseManager\ExpenseManager",
    [string]$OutputFile = "ExpenseManager_Diagram.puml"
)

$output = @"
@startuml ExpenseManager_Auto_Generated

"@

# Get all C# files
$csFiles = Get-ChildItem -Path $ProjectPath -Filter "*.cs" -Recurse | Where-Object { 
    $_.FullName -notlike "*\obj\*" -and 
    $_.FullName -notlike "*\bin\*" 
}

foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    
    # Extract namespace
    if ($content -match "namespace\s+([^\s{]+)") {
        $namespace = $matches[1]
        $output += "`nnamespace $namespace {`n"
    }
    
    # Extract class/interface definitions
    $classMatches = [regex]::Matches($content, "(?:public\s+)?(?:class|interface)\s+(\w+)")
    foreach ($match in $classMatches) {
        $className = $match.Groups[1].Value
        $output += "    class $className`n"
        
        # Extract public methods
        $methodMatches = [regex]::Matches($content, "public\s+[\w<>]+\s+(\w+)\s*\(")
        foreach ($methodMatch in $methodMatches) {
            $methodName = $methodMatch.Groups[1].Value
            if ($methodName -ne $className -and $methodName -ne "get" -and $methodName -ne "set") {
                $output += "        +$methodName()`n"
            }
        }
    }
    
    if ($content -match "namespace") {
        $output += "}`n"
    }
}

$output += "`n@enduml"

# Save to file
$output | Out-File -FilePath $OutputFile -Encoding UTF8
Write-Host "Diagram generated: $OutputFile"
Write-Host "Copy content to http://www.plantuml.com/plantuml/uml to visualize"