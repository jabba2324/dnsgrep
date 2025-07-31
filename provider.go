package main

// DNSProvider defines the interface for cloud provider DNS implementations
type DNSProvider interface {
	// SearchDomain searches for DNS records matching the given domain
	SearchDomain(domain string) error
}

// AWSProvider implements the DNSProvider interface for AWS Route53
type AWSProvider struct {
	Profile string // AWS profile name
}

// SearchDomain searches for DNS records in AWS Route53
func (p *AWSProvider) SearchDomain(domain string) error {
	searchDomainInProfile(p.Profile, domain)
	return nil
}