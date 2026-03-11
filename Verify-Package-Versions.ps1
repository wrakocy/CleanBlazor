# NOTES
# The Mediatr package has shifted from a free to a paid license. The paid
# license structure does not kick in until version 13, so for now, pin
# the package to the last 12.x release. This may in fact wind up being sufficient
# for the lifetime of the project, as we use the library for very basic tasks.
$expected = "12.5.0"
$pkg = "MediatR"
$assets = Get-ChildItem -Recurse -Filter project.assets.json -Path .\**\obj -ErrorAction SilentlyContinue

if (-not $assets) { Write-Error "No project.assets.json files found. Did restore run?"; exit 1 }

$fail = $false
foreach ($f in $assets) {
  $json = Get-Content $f.FullName -Raw | ConvertFrom-Json
  $libs = $json.libraries.PSObject.Properties | Where-Object { $_.Name -like "$pkg/*" }
  
  foreach ($lib in $libs) {
    $actual = ($lib.Name -split "/")[1]
    if ($actual -ne $expected) {
      Write-Host "$($f.FullName): $pkg resolved to $actual (expected $expected)"
      $fail = $true
    }
  }
}
if ($fail) { exit 1 } else { Write-Host "$pkg pinned to $expected everywhere."; exit 0 }