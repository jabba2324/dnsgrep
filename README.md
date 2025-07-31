# dnsgrep

A command-line tool to search AWS Route53 for DNS records matching a domain name.

## Installation

```bash
# Clone the repository
git clone https://github.com/jabba2324/dnsgrep.git
cd dnsgrep

# Download dependencies
go mod tidy

# Build the tool
go build -o dnsgrep

# Optional: Move to a directory in your PATH
mv dnsgrep /usr/local/bin/
```

## Usage

```bash
# Search for records matching example.com
dnsgrep -domain example.com

# Alternative syntax
dnsgrep example.com
```

## Features

- Automatically detects AWS profiles configured on your machine
- Searches all hosted zones in each profile for matching records
- Displays detailed record information including type, TTL, and values

## Requirements

- Go 1.16 or later
- AWS credentials configured (via AWS CLI or environment variables)
- Appropriate IAM permissions to list Route53 hosted zones and records

## AWS Permissions

The tool requires the following IAM permissions:
- `route53:ListHostedZones`
- `route53:ListResourceRecordSets`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.