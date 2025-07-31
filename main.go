package main

import (
	"flag"
	"fmt"
	"os"
	"strings"
)

func main() {
	// Parse command line arguments
	var domain string
	flag.StringVar(&domain, "domain", "", "Domain name to search for in DNS providers")
	flag.Parse()

	if domain == "" {
		if flag.NArg() > 0 {
			domain = flag.Arg(0)
		} else {
			fmt.Println("Error: Domain name is required")
			fmt.Println("Usage: dnsgrep -domain example.com")
			fmt.Println("   or: dnsgrep example.com")
			os.Exit(1)
		}
	}

	// Ensure domain ends with a dot for exact matching
	if !strings.HasSuffix(domain, ".") {
		domain = domain + "."
	}

	// Get available AWS profiles
	awsProfiles, err := getAWSProfiles()
	if err != nil {
		fmt.Printf("Error getting AWS profiles: %v\n", err)
		os.Exit(1)
	}

	if len(awsProfiles) == 0 {
		fmt.Println("No AWS profiles found. Please configure AWS CLI.")
		os.Exit(1)
	}

	fmt.Printf("Found %d AWS profile(s): %s\n", len(awsProfiles), strings.Join(awsProfiles, ", "))
	
	// Create providers for each AWS profile
	providers := make([]DNSProvider, 0, len(awsProfiles))
	for _, awsProfile := range awsProfiles {
		providers = append(providers, &AWSProvider{Profile: awsProfile})
	}
	
	// Search for domain in each provider
	for i, provider := range providers {
		awsProfile := awsProfiles[i]
		fmt.Printf("\nSearching in AWS profile: %s\n", awsProfile)
		provider.SearchDomain(domain)
	}
}

// getAWSProfiles returns a list of available AWS profiles
func getAWSProfiles() ([]string, error) {
	// Use our helper function to get profiles from config files
	return getAWSProfilesFromFiles()
}