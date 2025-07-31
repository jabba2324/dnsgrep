package main

import (
	"bufio"
	"os"
	"path/filepath"
	"regexp"
	"strings"
)

// getAWSProfilesFromFiles parses AWS config and credentials files to extract profile names
func getAWSProfilesFromFiles() ([]string, error) {
	profiles := make(map[string]bool)
	
	// Add default profile
	profiles["default"] = true
	
	// Check environment variables for config file locations
	configFile := os.Getenv("AWS_CONFIG_FILE")
	if configFile == "" {
		homeDir, err := os.UserHomeDir()
		if err == nil {
			configFile = filepath.Join(homeDir, ".aws", "config")
		}
	}
	
	credsFile := os.Getenv("AWS_SHARED_CREDENTIALS_FILE")
	if credsFile == "" {
		homeDir, err := os.UserHomeDir()
		if err == nil {
			credsFile = filepath.Join(homeDir, ".aws", "credentials")
		}
	}
	
	// Parse config file
	if configFile != "" {
		parseProfilesFromFile(configFile, profiles)
	}
	
	// Parse credentials file
	if credsFile != "" {
		parseProfilesFromFile(credsFile, profiles)
	}
	
	// Convert map to slice
	result := make([]string, 0, len(profiles))
	for profile := range profiles {
		result = append(result, profile)
	}
	
	return result, nil
}

// parseProfilesFromFile extracts profile names from an AWS config or credentials file
func parseProfilesFromFile(filePath string, profiles map[string]bool) {
	file, err := os.Open(filePath)
	if err != nil {
		return // Silently ignore if file doesn't exist
	}
	defer file.Close()
	
	// Regular expression to match profile sections
	// Matches both [profile name] (config file) and [name] (credentials file)
	profileRegex := regexp.MustCompile(`^\[(profile\s+)?([^\]]+)\]$`)
	
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		line := strings.TrimSpace(scanner.Text())
		matches := profileRegex.FindStringSubmatch(line)
		
		if len(matches) == 3 {
			profileName := matches[2]
			profiles[profileName] = true
		}
	}
}