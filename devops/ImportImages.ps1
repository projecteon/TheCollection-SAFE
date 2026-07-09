$ErrorActionPreference = "Stop"

# ------------------------------------------------------------
# Project-specific paths
# ------------------------------------------------------------

$AzCopy='.\azcopy_windows_amd64_10.30.1\azcopy'

$ImagesSource = "C:\Data\Theedatabase\Afbeeldingen Zakjes"
$ThumbnailsSource = "C:\Data\Theedatabase\thumbnails"

$ImagesContainer = "images"
$ThumbnailsContainer = "thumbnails"

# ------------------------------------------------------------
# Azurite localhost development storage
# ------------------------------------------------------------

$AccountName = "devstoreaccount1"

# Default Azurite development key
$DestinationKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="

$BlobEndpoint = "http://127.0.0.1:10000/devstoreaccount1"

$ConnectionString = "DefaultEndpointsProtocol=http;AccountName=$AccountName;AccountKey=$DestinationKey;BlobEndpoint=$BlobEndpoint;"

# ------------------------------------------------------------
# Validation
# ------------------------------------------------------------

if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw "Azure CLI 'az' was not found on PATH."
}

if (-not (Get-Command $AzCopy -ErrorAction SilentlyContinue)) {
    throw "AzCopy '$AzCopy' was not found on PATH."
}

if (-not (Test-Path -LiteralPath $ImagesSource -PathType Container)) {
    throw "Images source folder does not exist: $ImagesSource"
}

if (-not (Test-Path -LiteralPath $ThumbnailsSource -PathType Container)) {
    throw "Thumbnails source folder does not exist: $ThumbnailsSource"
}

Write-Host "Using AzCopy:"
& $AzCopy --version

# ------------------------------------------------------------
# Helpers
# ------------------------------------------------------------

function Ensure-ContainerExists {
    param(
        [Parameter(Mandatory)]
        [string]$ContainerName
    )

    Write-Host "Checking container '$ContainerName'..."

    $exists = az storage container exists `
        --name $ContainerName `
        --connection-string $ConnectionString `
        --query "exists" `
        -o tsv

    if ($LASTEXITCODE -ne 0) {
        throw "Failed checking whether container '$ContainerName' exists."
    }

    if ($exists -eq "true") {
        Write-Host "Container '$ContainerName' already exists."
        return
    }

    Write-Host "Creating container '$ContainerName'..."

    az storage container create `
        --name $ContainerName `
        --connection-string $ConnectionString `
        --public-access off `
        -o none

    if ($LASTEXITCODE -ne 0) {
        throw "Failed creating container '$ContainerName'."
    }
}

function New-AccountSasToken {
    Write-Host "Generating SAS token..."

    $expiry = (Get-Date).ToUniversalTime().AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ")

    $sas = az storage account generate-sas `
        --account-name $AccountName `
        --account-key $DestinationKey `
        --services b `
        --resource-types sco `
        --permissions racwdl `
        --expiry $expiry `
        -o tsv

    if ($LASTEXITCODE -ne 0) {
        throw "Failed generating SAS token."
    }

    if ([string]::IsNullOrWhiteSpace($sas)) {
        throw "SAS token is empty. Cancelling script."
    }

    return $sas
}

function Upload-FolderToContainer {
    param(
        [Parameter(Mandatory)]
        [string]$SourceFolder,

        [Parameter(Mandatory)]
        [string]$ContainerName,

        [Parameter(Mandatory)]
        [string]$SasToken
    )

    # Upload contents of folder, not the folder itself
    $sourcePattern = Join-Path $SourceFolder "*"

    $destinationUrl = "$BlobEndpoint/$ContainerName`?$SasToken"

    Write-Host ""
    Write-Host "Uploading to container '$ContainerName'"
    Write-Host "  From: $sourcePattern"
    Write-Host "  To:   $BlobEndpoint/$ContainerName?<sas>"
    Write-Host ""

    & $AzCopy copy `
        $sourcePattern `
        $destinationUrl `
        --recursive=true `
        --from-to=LocalBlob `
        --overwrite=true `
        --content-type="image/jpeg" `
        --cap-mbps 50

    if ($LASTEXITCODE -ne 0) {
        throw "AzCopy upload failed for container '$ContainerName'. Exit code: $LASTEXITCODE"
    }
}

# ------------------------------------------------------------
# Main
# ------------------------------------------------------------

Ensure-ContainerExists -ContainerName $ImagesContainer
Ensure-ContainerExists -ContainerName $ThumbnailsContainer


Write-Host "Set azcopy env vars"
$env:AZCOPY_CONCURRENCY_VALUE="32"
$env:AZCOPY_RETRY_DELAY="00:00:30"
$env:AZCOPY_RETRY_DURATION="01:00:00"

$sas = New-AccountSasToken

# Upload-FolderToContainer `
#     -SourceFolder $ImagesSource `
#     -ContainerName $ImagesContainer `
#     -SasToken $sas

Upload-FolderToContainer `
    -SourceFolder $ThumbnailsSource `
    -ContainerName $ThumbnailsContainer `
    -SasToken $sas

Write-Host ""
Write-Host "Upload completed successfully."