$path = Get-Location
Write-Host $path
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$($env:USERPROFILE)\Desktop\Cisco WLAN Guest Users Auto.lnk")
$Shortcut.TargetPath = "$path\Cisco WLAN Guest Users.exe"
$Shortcut.IconLocation = "$path\Cisco WLAN Guest Users.exe"
$Shortcut.Arguments = "auto"
$Shortcut.Hotkey = "CTRL+F12"
$Shortcut.Save()
Sleep -Seconds 10