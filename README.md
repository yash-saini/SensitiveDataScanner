# Sensitive Data Scan

A .NET MAUI application that analyzes text content for sentiment analysis and key phrase extraction, with database storage for scan results.

## Features

- **Text Analysis**: Analyze documents to get character count, word count, and line count
- **Sentiment Analysis**: ML.NET-powered sentiment analysis to determine if content is positive or negative
- **Key Phrase Extraction**: Identifies the most important phrases in your content
- **Local Database**: SQLite database storage for scan history and flagged content
- **Cross-Platform**: Works on Android, iOS, Windows, and macOS thanks to .NET MAUI

## Screenshots

*Add screenshots of your application here. For example:*

### Main Screen
![image](https://github.com/user-attachments/assets/57f18f33-863a-4954-bf5e-3a4e05305b69)


### Analysis Results
![image](https://github.com/user-attachments/assets/94ae8635-8124-45ef-b0d2-ed56f62dd452)


### Display Options
![image](https://github.com/user-attachments/assets/e0b43731-2335-4f66-8471-2fcc9e13ddbd)

## Sample Output

When you scan a document and export the results, the application generates a file like this:


<Original File: sample.txt

Detected Sensitive Items (Type | Original -> Masked):
john.doe@example.com -> ********@example.com
jane_smith@corporate.org -> **********@corporate.org
test.user@demo.co.uk -> *********@demo.co.uk
(123) 456-7890 -> **************
987-654-3210 -> ************
123.456.7890 -> ************
4111 1111 1111 1111 -> **** **** **** 1111
5500-0000-0000-0004 -> ****-****-****-0004
3400 000000 00009 -> **** ****** *0009
123-45-6789 -> ***-**-6789
987-65-4321 -> ***-**-4321

---- REDACTED CONTENT ----
Hello Team,

Please find below some test data containing sensitive information:

John Doe - SSN: ***-**-6789  
Jane Smith - SSN: ***-**-4321  

Contact Emails:  
- ********@example.com  
- **********@corporate.org  
- *********@demo.co.uk  

Phone Numbers:  
- **************  
- ************  
- ************  

Credit Card Numbers:  
- **** **** **** 1111  
- ****-****-****-0004  
- **** ****** *0009  

Non-sensitive filler text:
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed sit amet nunc at lorem efficitur suscipit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae.

Thanks,  
Security Team>

## Technical Details

### Machine Learning
The application uses ML.NET for machine learning capabilities:
- Sentiment analysis with SdcaLogisticRegression
- Local model training and persistence
- Asynchronous content processing

### Data Storage
- **Entity Framework Core** with SQLite provider for local data storage
- Database context for `ScannedFile` and `FlaggedItem` entities
- Relation mapping with one-to-many relationships
- LINQ queries for data filtering and manipulation

### Architecture
- MVVM pattern for UI separation
- Dependency Injection for services
- Asynchronous operations for performance
- Repository pattern for data access

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio 2022
3. Build and run the application

## How to Use

1. Launch the application
2. Paste or type text into the content area
3. Click "Analyze" to process the content
4. View the analysis results including sentiment score and key phrases
5. Results are automatically saved to the local database
6. Access scan history from the history tab

## Development

The project is built with:
- .NET 8
- .NET MAUI
- ML.NET
- Entity Framework Core
- SQLite database
- LINQ for data queries
