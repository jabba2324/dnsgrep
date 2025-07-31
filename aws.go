package main

import (
	"context"
	"fmt"
	"strings"

	"github.com/aws/aws-sdk-go-v2/aws"
	"github.com/aws/aws-sdk-go-v2/config"
	"github.com/aws/aws-sdk-go-v2/service/route53"
	"github.com/aws/aws-sdk-go-v2/service/route53/types"
)

// searchDomainInProfile searches for the domain in the specified AWS profile
func searchDomainInProfile(awsProfile, domain string) {
	ctx := context.Background()
	
	// Load AWS configuration with the specified profile
	cfg, err := config.LoadDefaultConfig(ctx, config.WithSharedConfigProfile(awsProfile))
	if err != nil {
		fmt.Printf("Error loading AWS config for profile %s: %v\n", awsProfile, err)
		return
	}
	
	// Create Route53 client
	r53Client := route53.NewFromConfig(cfg)
	
	// List hosted zones
	zonesOutput, err := r53Client.ListHostedZones(ctx, &route53.ListHostedZonesInput{})
	if err != nil {
		fmt.Printf("Error listing hosted zones: %v\n", err)
		return
	}
	
	foundRecords := false
	
	// Iterate through each hosted zone
	for _, zone := range zonesOutput.HostedZones {
		zoneName := aws.ToString(zone.Name)
		
		// Check if domain is in this zone
		if !strings.HasSuffix(domain, zoneName) && domain != zoneName {
			continue
		}
		
		// List records in the zone
		recordsOutput, err := r53Client.ListResourceRecordSets(ctx, &route53.ListResourceRecordSetsInput{
			HostedZoneId: zone.Id,
		})
		if err != nil {
			fmt.Printf("Error listing records for zone %s: %v\n", zoneName, err)
			continue
		}
		
		// Filter and display matching records
		for _, record := range recordsOutput.ResourceRecordSets {
			recordName := aws.ToString(record.Name)
			
			if strings.EqualFold(recordName, domain) || strings.HasSuffix(recordName, "."+domain) {
				if !foundRecords {
					fmt.Printf("Found matching records in zone: %s (ID: %s)\n", zoneName, aws.ToString(zone.Id))
					foundRecords = true
				}
				
				printRecord(record)
			}
		}
	}
	
	if !foundRecords {
		fmt.Printf("No matching records found for domain %s in AWS profile %s\n", domain, awsProfile)
	}
}

// printRecord prints a DNS record in a readable format
func printRecord(record types.ResourceRecordSet) {
	fmt.Printf("  %s\t%s\t", aws.ToString(record.Name), record.Type)
	
	if record.TTL != nil {
		fmt.Printf("TTL: %d\t", aws.ToInt64(record.TTL))
	}
	
	if len(record.ResourceRecords) > 0 {
		values := make([]string, len(record.ResourceRecords))
		for i, rr := range record.ResourceRecords {
			values[i] = aws.ToString(rr.Value)
		}
		fmt.Printf("%s", strings.Join(values, ", "))
	} else if record.AliasTarget != nil {
		fmt.Printf("ALIAS -> %s", aws.ToString(record.AliasTarget.DNSName))
	}
	
	fmt.Println()
}