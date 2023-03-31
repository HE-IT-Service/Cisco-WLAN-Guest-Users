<#
.SYNOPSIS
    Adds a local guest user to your Cisco WLC based wireless LAN via SNMPv2.
.DESCRIPTION
    This script requires the powershell SNMP module to be installed. (Install-Module -Name SNMP)

    With this script you can add a local guest user to your Cisco Wireless Lan Controller.

    You have to have SNMP write access to your WLC.
    Configure via WEB-UI: Advanced -> Management -> SNMP -> Communities

    Password for the guest user will be automatically generated with a length of 10 characters and consists of uppercase, lowercase and numbers.
    You can edit the line
    $GuestPass = New-Password -length 10 -Uppercase -LowerCase -Numeric
    if you want to add complexity.
    For example: $GuestPass = New-Password -length 20 -Uppercase -LowerCase -Numeric -Symbolic
    would create a password like 'jCd(e-$oQ+.H0T,E*8fh'

    This script does not allow to add permanent guest users since the LifeTime parameter is mandatory and expects a value between 5 minutes and 30 days.
    If you wish to allow permanent users, remove the validation [ValidateRange(5,43200)] and run the script with -GuestLifeTimeMinute 0

.NOTES
    Author: Michael Reiner
    Date:   July 25, 2017

    Tested with Cisco WLC 2500 Series Controller, FW Version 8.0.120.23
#>
Param(
    #Cisco Wireless Lan Controller IP
    [Parameter(Mandatory=$True)]
    [ValidateScript({$_ -match [IPAddress]$_ })]
    [string]$IP,

    #SNMP Write Community
    [Parameter(Mandatory=$True)]
    [string]$Community,

    #ID of guest WLAN (check via SSH with: show wlan summary)
    [Parameter(Mandatory=$True)]
    [int]$WLanID,

    #Guest Username
    [Parameter(Mandatory=$True)]
    [string]$GuestUser,

    #Guest Password
    [Parameter(Mandatory=$True)]
    [string]$GuestPass,

    #Guest User Description
    [Parameter(Mandatory=$True)]
    [string]$GuestDesc,

    #User Lifetime in Minutes. Min: 5, Max: 43.200 (1440 = 1 day)
    [Parameter(Mandatory=$True)]
    [ValidateRange(5,43200)]
    [int]$GuestLifeTimeMinute
)

Function Get-MyModule 
{ 
    Param([string]$name) 
    if(-not(Get-Module -name $name)) 
    { 
        if(Get-Module -ListAvailable | 
        Where-Object { $_.name -eq $name }) 
        { 
            Import-Module -Name $name 
            $true 
        }
        else { $false }
        }
    else { $true }
}

Function New-Password { 
 
    [CmdletBinding()] 
    [OutputType([String])] 
 
     
    Param( 
 
        [int]$length=30, 
 
        [alias("U")] 
        [Switch]$Uppercase, 
 
        [alias("L")] 
        [Switch]$LowerCase, 
 
        [alias("N")] 
        [Switch]$Numeric, 
 
        [alias("S")] 
        [Switch]$Symbolic 
 
    ) 
 
    Begin {} 
 
    Process { 
         
        If ($Uppercase) {$CharPool += ([char[]](64..90))} 
        If ($LowerCase) {$CharPool += ([char[]](97..122))} 
        If ($Numeric) {$CharPool += ([char[]](48..57))} 
        If ($Symbolic) {$CharPool += ([char[]](33..47)) 
                       $CharPool += ([char[]](33..47))} 
         
        If ($CharPool -eq $null) { 
            Throw 'You must select at least one of the parameters "Uppercase" "LowerCase" "Numeric" or "Symbolic"' 
        } 
 
        [String]$Password =  (Get-Random -InputObject $CharPool -Count $length) -join '' 
 
    } 
     
    End { 
         
        return $Password 
     
    } 
}

#Convert LifeTime to Seconds x 100
$GuestLifeTime = $GuestLifeTimeMinute * 60 * 100
#Generate Password
#$GuestPass = New-Password -length 10 -Uppercase -LowerCase -Numeric
#Char Count of Username
$GuestUserLength = $GuestUser.Length
#Convert the chars of the username to their corresponding ASCII numbers
$GuestUserChars = $GuestUser.ToCharArray() | %{[int][char]$_}
#Create OID appendix for User. All OIDs have to be in format of BaseOID.username-length.username-char1.username-char2.username-char3.etc.etc...
$OIDGuestUser = ''
foreach ($GuestUserChar in $GuestUserChars){
    $OIDGuestUser += '.' + $GuestUserChar
}


#This section sets the base OIDs for our needed functions
$BaseOIDCheckUser = '.1.3.6.1.4.1.14179.2.5.10.1.24' #If this returns something, the user does already exist. Otherwise create with Value '4'
$BaseOIDWLanID = '.1.3.6.1.4.1.14179.2.5.10.1.2'
$BaseOIDGuestPass = '.1.3.6.1.4.1.14179.2.5.10.1.3'
$BaseOIDGuestDesc = '.1.3.6.1.4.1.14179.2.5.10.1.4'
$BaseOIDGuestLifeTime = '.1.3.6.1.4.1.14179.2.5.10.1.5'

#If this is set to Value '1', the added user will be a guest user
$BaseOIDLocalNetUserIsGuest = '.1.3.6.1.4.1.9.9.515.2.3.1.1.2'

#Append username-length and ASCII numbers of username chars to base OIDs
$OIDCheckUser = $BaseOIDCheckUser + '.' + $GuestUserLength + $OIDGuestUser
$OIDWLanID = $BaseOIDWLanID + '.' + $GuestUserLength + $OIDGuestUser
$OIDGuestPass = $BaseOIDGuestPass + '.' + $GuestUserLength + $OIDGuestUser
$OIDGuestDesc = $BaseOIDGuestDesc + '.' + $GuestUserLength + $OIDGuestUser
$OIDGuestLifeTime = $BaseOIDGuestLifeTime + '.' + $GuestUserLength + $OIDGuestUser
$OIDLocalNetUserIsGuest = $BaseOIDLocalNetUserIsGuest + '.' + $GuestUserLength + $OIDGuestUser

$CheckUser = ''
$Error.Clear()

if(Get-MyModule -name "SNMP") #Check if powershell SNMP module is installed and loaded.
{
    try {
        $CheckUser = ([string](Get-SnmpData -IP $IP -OID $OIDCheckUser -Community $Community -Version V2).Data) #Check if guest user already exists
    }
    catch
	{
        write-host "Exception Type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
        write-host "Exception Message: $($_.Exception.Message)" -ForegroundColor Red
    }
    if (!$Error)
    {
        if ($CheckUser -eq 'NoSuchInstance'){
            Set-SnmpData -IP $IP -OID $OIDCheckUser -Community $Community -Version V2 -Value '4' -ValueType Integer32
            Start-Sleep -Seconds 2
            Set-SnmpData -IP $IP -OID $OIDWLanID -Community $Community -Version V2 -Value $WLanID -ValueType Integer32
            Start-Sleep -Seconds 2
            Set-SnmpData -IP $IP -OID $OIDGuestPass -Community $Community -Version V2 -Value $GuestPass -ValueType OctetString
            Start-Sleep -Seconds 2
            Set-SnmpData -IP $IP -OID $OIDGuestDesc -Community $Community -Version V2 -Value $GuestDesc -ValueType OctetString
            Start-Sleep -Seconds 2
            Set-SnmpData -IP $IP -OID $OIDGuestLifeTime -Community $Community -Version V2 -Value $GuestLifeTime -ValueType Integer32
            Start-Sleep -Seconds 2
            Set-SnmpData -IP $IP -OID $OIDLocalNetUserIsGuest -Community $Community -Version V2 -Value '1' -ValueType Integer32
			return "Password: $($GuestPass)"
        }
        elseif ($CheckUser -eq 'NoSuchObject')
		{
            write-host 'SNMP returned NoSuchObject. Check your SNMP settings.' -ForegroundColor Red
        }
        else
		{
            write-host 'Username already exists, or there may be a problem with SNMP' -ForegroundColor Red
        }
    }
}
else
{
    Throw 'SNMP Module not installed. Please run: Install-Module -Name SNMP';
	exit;
}